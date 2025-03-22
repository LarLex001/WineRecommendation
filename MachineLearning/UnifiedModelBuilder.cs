using Microsoft.ML;
using WineRecommendation.Models;

namespace WineRecommendation.MachineLearning
{
    public static class UnifiedModelBuilder
    {
        private static readonly string TypeModelPath = "MachineLearning/Models/wine_type_model.zip";
        private static readonly string QualityModelPath = "MachineLearning/Models/wine_quality_model.zip";
        private static readonly MLContext MlContext = new MLContext(seed: 0);

        // classes for ML.NET predictions
        private class TypePrediction
        {
            [Microsoft.ML.Data.ColumnName("PredictedLabel")]
            public string PredictedType { get; set; } = string.Empty;
        }

        private class QualityPrediction
        {
            [Microsoft.ML.Data.ColumnName("Score")]
            public float PredictedQuality { get; set; }
        }

        public static void TrainTypeModel(IEnumerable<WineData> trainingData)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(TypeModelPath)!);

            var dataView = MlContext.Data.LoadFromEnumerable(trainingData);

            var pipeline = MlContext.Transforms.Conversion.MapValueToKey("Label", nameof(WineData.Type))
                .Append(MlContext.Transforms.Concatenate("Features",
                    nameof(WineData.Alcohol),
                    nameof(WineData.PH),
                    nameof(WineData.ResidualSugar),
                    nameof(WineData.FixedAcidity),
                    nameof(WineData.VolatileAcidity),
                    nameof(WineData.CitricAcid),
                    nameof(WineData.Chlorides),
                    nameof(WineData.Density)))
                .Append(MlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
                .Append(MlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            var model = pipeline.Fit(dataView);

            MlContext.Model.Save(model, dataView.Schema, TypeModelPath);
            Console.WriteLine("Type prediction model trained and saved.");
        }

        public static void TrainQualityModel(IEnumerable<WineData> trainingData)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(QualityModelPath)!);

            var dataView = MlContext.Data.LoadFromEnumerable(trainingData);

            var pipeline = MlContext.Transforms.Concatenate("Features",
                    nameof(WineData.Alcohol),
                    nameof(WineData.PH),
                    nameof(WineData.ResidualSugar),
                    nameof(WineData.VolatileAcidity),
                    nameof(WineData.CitricAcid),
                    nameof(WineData.Sulphates),
                    nameof(WineData.FixedAcidity),
                    nameof(WineData.Chlorides),
                    nameof(WineData.Density))
                .Append(MlContext.Regression.Trainers.FastTree(
                    labelColumnName: nameof(WineData.Quality),
                    featureColumnName: "Features"));

            var model = pipeline.Fit(dataView);

            MlContext.Model.Save(model, dataView.Schema, QualityModelPath);
            Console.WriteLine("Quality prediction model trained and saved.");
        }

        public static string PredictType(WineInputModel input)
        {
            if (!File.Exists(TypeModelPath))
                throw new FileNotFoundException("Type prediction model not found. Please train the model first.");

            var model = MlContext.Model.Load(TypeModelPath, out _);

            var wineData = new WineData
            {
                Alcohol = input.Alcohol,
                PH = input.PH,
                ResidualSugar = input.ResidualSugar,
                FixedAcidity = input.FixedAcidity,
                VolatileAcidity = input.VolatileAcidity,
                CitricAcid = input.CitricAcid,
                Chlorides = input.Chlorides,
                Density = input.Density,
                Sulphates = input.Sulphates,
                FreeSulfurDioxide = input.FreeSulfurDioxide,
                TotalSulfurDioxide = input.TotalSulfurDioxide,
                Type = "Unknown"
            };

            var predictionEngine = MlContext.Model.CreatePredictionEngine<WineData, TypePrediction>(model);

            var prediction = predictionEngine.Predict(wineData);
            return prediction.PredictedType;
        }

        public static float PredictQuality(WineInputModel input)
        {
            if (!File.Exists(QualityModelPath))
                throw new FileNotFoundException("Quality prediction model not found. Please train the model first.");

            var model = MlContext.Model.Load(QualityModelPath, out _);

            var predictionEngine = MlContext.Model.CreatePredictionEngine<WineInputModel, QualityPrediction>(model);

            var prediction = predictionEngine.Predict(input);
            return prediction.PredictedQuality;
        }

        public static void EvaluateTypeModel(IEnumerable<WineData> testData)
        {
            if (!File.Exists(TypeModelPath))
            {
                Console.WriteLine("Type model not found. Please train the model first.");
                return;
            }

            var model = MlContext.Model.Load(TypeModelPath, out var modelSchema);

            var testDataView = MlContext.Data.LoadFromEnumerable(testData);

            var dataPrepPipeline = MlContext.Transforms.Conversion.MapValueToKey("Label", nameof(WineData.Type));
            var transformedData = dataPrepPipeline.Fit(testDataView).Transform(testDataView);

            var predictions = model.Transform(transformedData);
            var metrics = MlContext.MulticlassClassification.Evaluate(predictions);

            Console.WriteLine("=== Wine Type Model Evaluation ===");
            Console.WriteLine($"Micro Accuracy: {metrics.MicroAccuracy:F4}");
            Console.WriteLine($"Macro Accuracy: {metrics.MacroAccuracy:F4}");
        }

        public static void EvaluateQualityModel(IEnumerable<WineData> testData)
        {
            if (!File.Exists(QualityModelPath))
            {
                Console.WriteLine("Quality model not found. Please train the model first.");
                return;
            }

            var model = MlContext.Model.Load(QualityModelPath, out var modelSchema);

            var testDataView = MlContext.Data.LoadFromEnumerable(testData);

            var dataPrepPipeline = MlContext.Transforms.CopyColumns("Label", nameof(WineData.Quality));
            var transformedData = dataPrepPipeline.Fit(testDataView).Transform(testDataView);

            var predictions = model.Transform(transformedData);
            var metrics = MlContext.Regression.Evaluate(predictions, labelColumnName: "Label");

            Console.WriteLine("=== Wine Quality Model Evaluation ===");
            Console.WriteLine($"Mean Absolute Error: {metrics.MeanAbsoluteError:F4}");
            Console.WriteLine($"Mean Squared Error: {metrics.MeanSquaredError:F4}");
            Console.WriteLine($"R-Squared: {metrics.RSquared:F4}");
        }
    }
}