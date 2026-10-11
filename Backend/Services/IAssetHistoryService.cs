using Backend.DTOs;

namespace Backend.Services
{
    public interface IAssetHistoryService
    {
        Task<IEnumerable<AssetHistoryDto>> GetAllHistoriesAsync();
        Task<IEnumerable<AssetHistoryDto>> GetHistoryByAssetIdAsync(int assetId);
    }
}
