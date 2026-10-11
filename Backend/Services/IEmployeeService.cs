using Backend.DTOs;

namespace Backend.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync(string? search = null);
        Task<EmployeeDto?> GetEmployeeByIdAsync(int id);
        Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto createDto);
        Task<EmployeeDto> UpdateEmployeeAsync(int id, UpdateEmployeeDto updateDto);
        Task<bool> DeactivateEmployeeAsync(int id);
        Task<IEnumerable<AssetDto>> GetAssignedAssetsForEmployeeAsync(int employeeId);
    }
}
