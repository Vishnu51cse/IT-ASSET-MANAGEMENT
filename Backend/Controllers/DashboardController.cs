using System.Security.Claims;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            var metrics = await _dashboardService.GetAdminDashboardMetricsAsync();
            return Ok(metrics);
        }

        [HttpGet("employee")]
        public async Task<IActionResult> GetEmployeeDashboard()
        {
            var empClaim = User.FindFirst("EmployeeId");
            if (empClaim == null || !int.TryParse(empClaim.Value, out int employeeId))
            {
                // Fallback to employeeId 1 for testing if admin calls employee dashboard
                employeeId = 1;
            }

            var metrics = await _dashboardService.GetEmployeeDashboardMetricsAsync(employeeId);
            return Ok(metrics);
        }
    }
}
