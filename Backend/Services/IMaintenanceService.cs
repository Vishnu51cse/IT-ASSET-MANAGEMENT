using Backend.DTOs;

namespace Backend.Services
{
    public interface IMaintenanceService
    {
        Task<IEnumerable<MaintenanceDto>> GetAllMaintenanceRecordsAsync(int? assetId = null, string? status = null);
        Task<MaintenanceDto?> GetMaintenanceByIdAsync(int id);
        Task<MaintenanceDto> CreateMaintenanceAsync(CreateMaintenanceDto dto, string performedBy);
        Task<MaintenanceDto> CompleteMaintenanceAsync(int id, CompleteMaintenanceDto dto, string performedBy);
    }
}
