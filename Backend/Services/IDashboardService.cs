using Backend.DTOs;

namespace Backend.Services
{
    public interface IDashboardService
    {
        Task<AdminDashboardDto> GetAdminDashboardMetricsAsync();
        Task<EmployeeDashboardDto> GetEmployeeDashboardMetricsAsync(int employeeId);
    }
}
