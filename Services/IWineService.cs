using WineRecommendation.Models;

namespace WineRecommendation.Services
{
    public interface IWineService
    {
        Task<List<WineData>> GetAllWinesAsync();
        Task<WineData?> GetWineByIdAsync(int id);
        Task<WineData> AddWineAsync(WineData wine);
        Task UpdateWineAsync(WineData wine);
        Task DeleteWineAsync(int id);
        Task<int> GetTotalRecordsCountAsync();
        Task<List<WineData>> GetWinePageAsync(int page, int pageSize);
    }
}