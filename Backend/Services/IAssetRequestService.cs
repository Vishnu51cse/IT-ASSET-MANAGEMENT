using Backend.DTOs;

namespace Backend.Services
{
    public interface IAssetRequestService
    {
        Task<IEnumerable<AssetRequestDto>> GetAllRequestsAsync(int? employeeId = null, string? status = null);
        Task<AssetRequestDto?> GetRequestByIdAsync(int id);
        Task<AssetRequestDto> CreateRequestAsync(int employeeId, CreateAssetRequestDto dto);
        Task<AssetRequestDto> ProcessRequestAsync(int requestId, ProcessRequestDto dto, string adminName);
    }
}
