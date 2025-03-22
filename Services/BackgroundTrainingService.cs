using Microsoft.EntityFrameworkCore;
using WineRecommendation.Data;

namespace WineRecommendation.Services
{
    public class BackgroundTrainingService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BackgroundTrainingService> _logger;
        private readonly BackgroundTrainingQueue _trainingQueue;

        public BackgroundTrainingService(
            IServiceProvider serviceProvider,
            ILogger<BackgroundTrainingService> logger,
            BackgroundTrainingQueue trainingQueue)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _trainingQueue = trainingQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Background Training Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var trainingItem = await _trainingQueue.DequeueAsync(stoppingToken);

                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var modelTrainingService = scope.ServiceProvider.GetRequiredService<ModelTrainingService>();
                        var dbContext = scope.ServiceProvider.GetRequiredService<WineDbContext>();

                        _logger.LogInformation("Starting background model retraining with {Count} predictions", trainingItem.PredictionIds.Count);

                        var predictions = await dbContext.PredictionResults
                            .Where(p => trainingItem.PredictionIds.Contains(p.Id))
                            .ToListAsync();

                        await modelTrainingService.RetrainModelsWithNewDataAsync(predictions);
                        await modelTrainingService.MarkPredictionsAsTrainedAsync(trainingItem.PredictionIds);

                        _logger.LogInformation("Background model retraining completed successfully");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during background model retraining");
                }
            }

            _logger.LogInformation("Background Training Service is stopping.");
        }
    }
}