using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using WineRecommendation.Data;
using WineRecommendation.MachineLearning;
using WineRecommendation.Models;

namespace WineRecommendation.Services
{
    public class ModelTrainingService
    {
        private readonly WineDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;

        public ModelTrainingService(WineDbContext dbContext, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            _environment = environment;
        }

        public async Task ImportInitialDataIfNeeded()
        {
            if (await _dbContext.Wines.AnyAsync()) return;
            
            var appDataPath = Path.Combine(_environment.ContentRootPath, "AppData");

            var redWineFile = Path.Combine(appDataPath, "red_wine.csv");
            var whiteWineFile = Path.Combine(appDataPath, "white_wine.csv");

            var redWines = new List<WineData>();
            var whiteWines = new List<WineData>();

            if (File.Exists(redWineFile)) 
                redWines = await ImportWineDataFromCsvAsync(redWineFile, "Red");
            
            if (File.Exists(whiteWineFile)) 
                whiteWines = await ImportWineDataFromCsvAsync(whiteWineFile, "White");

            int minCount = Math.Min(redWines.Count, whiteWines.Count);
            if (minCount == 0) return; 

            var random = new Random(42); 
            var balancedRedWines = redWines
                .OrderBy(x => random.Next())
                .Take(minCount)
                .ToList();
            
            var balancedWhiteWines = whiteWines
                .OrderBy(x => random.Next())
                .Take(minCount)
                .ToList();

            var allBalancedWines = balancedRedWines.Concat(balancedWhiteWines).ToList();
            
            if (allBalancedWines.Any())
            {
                await _dbContext.Wines.AddRangeAsync(allBalancedWines);
                await _dbContext.SaveChangesAsync();
                Console.WriteLine($"Imported {balancedRedWines.Count} Red wines and {balancedWhiteWines.Count} White wines (balanced)");
            }
        }

        private async Task<List<WineData>> ImportWineDataFromCsvAsync(string filePath, string wineType)
        {
            var wines = new List<WineData>();

            await Task.Run(async () => {
                using var reader = new StreamReader(filePath);

                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                    HeaderValidated = null,
                    MissingFieldFound = null,
                    HasHeaderRecord = true
                };

                using var csv = new CsvReader(reader, csvConfig);
                await csv.ReadAsync();
                csv.ReadHeader();

                while (await csv.ReadAsync())
                {
                    try
                    {
                        var wine = new WineData
                        {
                            FixedAcidity = csv.GetField<float>("fixed acidity"),
                            VolatileAcidity = csv.GetField<float>("volatile acidity"),
                            CitricAcid = csv.GetField<float>("citric acid"),
                            ResidualSugar = csv.GetField<float>("residual sugar"),
                            Chlorides = csv.GetField<float>("chlorides"),
                            FreeSulfurDioxide = csv.GetField<float>("free sulfur dioxide"),
                            TotalSulfurDioxide = csv.GetField<float>("total sulfur dioxide"),
                            Density = csv.GetField<float>("density"),
                            PH = csv.GetField<float>("pH"),
                            Sulphates = csv.GetField<float>("sulphates"),
                            Alcohol = csv.GetField<float>("alcohol"),
                            Quality = csv.GetField<float>("quality"),
                            Type = wineType,
                            IsTrainingData = true,
                            CreatedDate = DateTime.Now
                        };

                        wines.Add(wine);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error importing wine data: {ex.Message}");
                    }
                }
            });

            return wines;
        }

        public async Task EnsureModelsCreated()
        {
            string typeModelPath = Path.Combine(_environment.ContentRootPath, "MachineLearning", "Models", "wine_type_model.zip");
            string qualityModelPath = Path.Combine(_environment.ContentRootPath, "MachineLearning", "Models", "wine_quality_model.zip");

            if (!File.Exists(typeModelPath) || !File.Exists(qualityModelPath))
            {
                var trainingData = await _dbContext.Wines
                    .Where(w => w.IsTrainingData)
                    .ToListAsync();

                if (trainingData.Any())
                {
                    UnifiedModelBuilder.TrainTypeModel(trainingData);
                    UnifiedModelBuilder.TrainQualityModel(trainingData);
                }
            }
        }

        public async Task RetrainModelsWithNewDataAsync(IEnumerable<WinePredictionResult> newPredictions)
        {
            var newTrainingData = new List<WineData>();

            foreach (var prediction in newPredictions)
            {
                var wineData = new WineData
                {
                    FixedAcidity = prediction.FixedAcidity,
                    VolatileAcidity = prediction.VolatileAcidity,
                    CitricAcid = prediction.CitricAcid,
                    ResidualSugar = prediction.ResidualSugar,
                    Chlorides = prediction.Chlorides,
                    FreeSulfurDioxide = prediction.FreeSulfurDioxide,
                    TotalSulfurDioxide = prediction.TotalSulfurDioxide,
                    Density = prediction.Density,
                    PH = prediction.PH,
                    Sulphates = prediction.Sulphates,
                    Alcohol = prediction.Alcohol,
                    Quality = (float)Math.Round(prediction.PredictedQuality, 1),
                    Type = prediction.PredictedType,
                    IsTrainingData = true,
                    CreatedDate = DateTime.Now
                };

                newTrainingData.Add(wineData);
            }

            await _dbContext.Wines.AddRangeAsync(newTrainingData);
            await _dbContext.SaveChangesAsync();

            var allTrainingData = await _dbContext.Wines
                .Where(w => w.IsTrainingData)
                .ToListAsync();

            UnifiedModelBuilder.TrainTypeModel(allTrainingData);
            UnifiedModelBuilder.TrainQualityModel(allTrainingData);
            await EvaluateModelsAsync();

            Console.WriteLine($"Models retrained with {newPredictions.Count()} new data points");

        }

        public async Task EvaluateModelsAsync()
        {
            var allWines = await _dbContext.Wines.ToListAsync();
            var testDataCount = (int)(allWines.Count * 0.2);
            var testData = allWines.Take(testDataCount).ToList();

            UnifiedModelBuilder.EvaluateTypeModel(testData);
            UnifiedModelBuilder.EvaluateQualityModel(testData);
        }

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