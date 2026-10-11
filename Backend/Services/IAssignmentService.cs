using Backend.DTOs;

namespace Backend.Services
{
    public interface IAssignmentService
    {
        Task<IEnumerable<AssignmentDto>> GetAllAssignmentsAsync(int? employeeId = null, string? status = null);
        Task<AssignmentDto> AssignAssetAsync(AssignAssetDto dto, string performedBy);
        Task<AssignmentDto> ReturnAssetAsync(ReturnAssetDto dto, string performedBy);
    }
}
