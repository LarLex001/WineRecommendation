using Microsoft.EntityFrameworkCore;
using WineRecommendation.Data;
using WineRecommendation.MachineLearning;
using WineRecommendation.Models;

namespace WineRecommendation.Services
{
    public class WinePredictionService : IWinePredictionService
    {
        private readonly WineDbContext _dbContext;
        private readonly ModelTrainingService _modelTrainingService;

        public WinePredictionService(WineDbContext dbContext, ModelTrainingService modelTrainingService)
        {
            _dbContext = dbContext;
            _modelTrainingService = modelTrainingService;
        }

        public async Task<WinePredictionResult> PredictWineAsync(WineInputModel inputModel)
        {
            var predictionResult = new WinePredictionResult
            {
                FixedAcidity = inputModel.FixedAcidity,
                VolatileAcidity = inputModel.VolatileAcidity,
                CitricAcid = inputModel.CitricAcid,
                ResidualSugar = inputModel.ResidualSugar,
                Chlorides = inputModel.Chlorides,
                FreeSulfurDioxide = inputModel.FreeSulfurDioxide,
                TotalSulfurDioxide = inputModel.TotalSulfurDioxide,
                Density = inputModel.Density,
                PH = inputModel.PH,
                Sulphates = inputModel.Sulphates,
                Alcohol = inputModel.Alcohol,
                PredictionDate = DateTime.Now
            };

            predictionResult.PredictedType = UnifiedModelBuilder.PredictType(inputModel);
            predictionResult.PredictedQuality = UnifiedModelBuilder.PredictQuality(inputModel);

            _dbContext.PredictionResults.Add(predictionResult);
            await _dbContext.SaveChangesAsync();

            var untrainedCount = await GetUntrainedPredictionsCountAsync();
            if (untrainedCount >= 10)
            {
                var untrainedPredictions = await _dbContext.PredictionResults
                    .Where(p => !p.ContributedToRetraining)
                    .ToListAsync();

                await _modelTrainingService.RetrainModelsWithNewDataAsync(untrainedPredictions);

                var predictionIds = untrainedPredictions.Select(p => p.Id);
                await MarkPredictionsAsTrainedAsync(predictionIds);
            }

            return predictionResult;
        }

        public async Task<WinePredictionResult?> GetPredictionByIdAsync(int id) => 
            await _dbContext.PredictionResults.FindAsync(id);
        
        public async Task<List<WinePredictionResult>> GetAllPredictionsAsync() => 
            await _dbContext.PredictionResults
                            .OrderByDescending(p => p.PredictionDate)
                            .ToListAsync();

        public async Task<int> GetTotalPredictionsAsync() => await _dbContext.PredictionResults.CountAsync();

        public async Task<int> GetUntrainedPredictionsCountAsync() =>
            await _dbContext.PredictionResults
                            .Where(p => !p.ContributedToRetraining)
                            .CountAsync();
 
        public async Task MarkPredictionsAsTrainedAsync(IEnumerable<int> predictionIds)
        {
            foreach (var id in predictionIds)
            {
                var prediction = await _dbContext.PredictionResults.FindAsync(id);
                if (prediction != null)
                {
                    prediction.ContributedToRetraining = true;
                    _dbContext.Entry(prediction).State = EntityState.Modified;
                }
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}