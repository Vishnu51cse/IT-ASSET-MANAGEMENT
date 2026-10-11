using Backend.DTOs;

namespace Backend.Services
{
    public interface IAssetService
    {
        Task<IEnumerable<AssetDto>> GetAssetsAsync(string? search = null, int? categoryId = null, string? status = null);
        Task<AssetDto?> GetAssetByIdAsync(int id);
        Task<AssetDto> CreateAssetAsync(CreateAssetDto createDto, string performedBy);
        Task<AssetDto> ProcureAndAssignAssetAsync(ProcureAndAssignAssetDto dto, string performedBy);
        Task<AssetDto> UpdateAssetAsync(int id, UpdateAssetDto updateDto, string performedBy);
        Task<bool> DeleteAssetAsync(int id, string performedBy);
    }
}
