using Microsoft.EntityFrameworkCore;
using WineRecommendation.Data;

namespace WineRecommendation.Services
{
    public class DatabaseService
    {
        private readonly WineDbContext _dbContext;

        public DatabaseService(WineDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task ResetDatabase()
        {
            await _dbContext.PredictionResults.ExecuteDeleteAsync();
            await _dbContext.Wines.ExecuteDeleteAsync();

            await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM sqlite_sequence WHERE name='PredictionResults'");
            await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM sqlite_sequence WHERE name='Wines'");

            await _dbContext.SaveChangesAsync();
        }
    }
}