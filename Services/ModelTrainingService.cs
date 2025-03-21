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
            
            var appDataPath = Path.Combine(_environment.ContentRootPath, "App_Data");

            var redWineFile = Path.Combine(appDataPath, "red_wine.csv");
            var whiteWineFile = Path.Combine(appDataPath, "white_wine.csv");

            if (File.Exists(redWineFile)) await ImportWineDataFromCsvAsync(redWineFile, "Red");
            if (File.Exists(whiteWineFile)) await ImportWineDataFromCsvAsync(whiteWineFile, "White");   
        }

        private async Task ImportWineDataFromCsvAsync(string filePath, string wineType)
        {
            using var reader = new StreamReader(filePath);

            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HeaderValidated = null,
                MissingFieldFound = null,
                HasHeaderRecord = true
            };

            using var csv = new CsvReader(reader, csvConfig);
            csv.Read();
            csv.ReadHeader();

            var wines = new List<WineData>();

            while (csv.Read())
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

            if (wines.Any())
            {
                await _dbContext.Wines.AddRangeAsync(wines);
                await _dbContext.SaveChangesAsync();
                Console.WriteLine($"Imported {wines.Count} {wineType} wines");
            }
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
    }
}