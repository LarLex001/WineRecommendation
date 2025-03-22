using System.Collections.Concurrent;

namespace WineRecommendation.Services
{
    public class TrainingItem
    {
        public List<int> PredictionIds { get; set; } = new List<int>();
    }

    public class BackgroundTrainingQueue
    {
        private readonly ConcurrentQueue<TrainingItem> _items = new ConcurrentQueue<TrainingItem>();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void QueueTrainingWork(TrainingItem trainingItem)
        {
            if (trainingItem == null)
                throw new ArgumentNullException(nameof(trainingItem));
            
            _items.Enqueue(trainingItem);
            _signal.Release();
        }

        public async Task<TrainingItem> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _items.TryDequeue(out var item);

            return item;
        }
    }
}