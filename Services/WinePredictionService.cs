using Microsoft.EntityFrameworkCore;
using WineRecommendation.Data;
using WineRecommendation.MachineLearning;
using WineRecommendation.Models;

namespace WineRecommendation.Services
{
    public class WinePredictionService : IWinePredictionService
    {
        private readonly WineDbContext _dbContext;
        private readonly BackgroundTrainingQueue _trainingQueue;
        private readonly ILogger<WinePredictionService> _logger;

        public WinePredictionService(
            WineDbContext dbContext,
            BackgroundTrainingQueue trainingQueue,
            ILogger<WinePredictionService> logger)
        {
            _dbContext = dbContext;
            _trainingQueue = trainingQueue;
            _logger = logger;
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
                var untrainedPredictionIds = await _dbContext.PredictionResults
                    .Where(p => !p.ContributedToRetraining)
                    .Select(p => p.Id)
                    .ToListAsync();

                _logger.LogInformation("Queueing background model retraining with {Count} predictions", untrainedPredictionIds.Count);
                _trainingQueue.QueueTrainingWork(new TrainingItem { PredictionIds = untrainedPredictionIds });
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
    }
}