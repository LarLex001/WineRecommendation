namespace WineRecommendation.Services
{
    public class SeedDataService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public SeedDataService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var modelTrainingService = scope.ServiceProvider.GetRequiredService<ModelTrainingService>();

                await modelTrainingService.ImportInitialDataIfNeeded();
                await modelTrainingService.EnsureModelsCreated();
                await modelTrainingService.EvaluateModelsAsync();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;  
    }
}