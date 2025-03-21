using Microsoft.EntityFrameworkCore;
using WineRecommendation.Data;
using WineRecommendation.Models;

namespace WineRecommendation.Services
{
    public class WineService : IWineService
    {
        private readonly WineDbContext _dbContext;

        public WineService(WineDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<WineData>> GetAllWinesAsync() =>
            await _dbContext.Wines.OrderByDescending(w => w.CreatedDate).ToListAsync();

        public async Task<WineData?> GetWineByIdAsync(int id) => 
            await _dbContext.Wines.FindAsync(id);

        public async Task<int> GetTotalRecordsCountAsync() => 
            await _dbContext.Wines.CountAsync();
        
        public async Task<WineData> AddWineAsync(WineData wine)
        {
            _dbContext.Wines.Add(wine);
            await _dbContext.SaveChangesAsync();
            return wine;
        }

        public async Task UpdateWineAsync(WineData wine)
        {
            _dbContext.Entry(wine).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteWineAsync(int id)
        {
            var wine = await _dbContext.Wines.FindAsync(id);
            if (wine != null)
            {
                _dbContext.Wines.Remove(wine);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<WineData>> GetWinePageAsync(int page, int pageSize) => 
            await _dbContext.Wines
                            .OrderByDescending(w => w.CreatedDate)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();     
    }
}