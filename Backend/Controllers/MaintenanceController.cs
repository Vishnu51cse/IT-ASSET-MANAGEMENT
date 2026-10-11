using System.Security.Claims;
using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MaintenanceController : ControllerBase
    {
        private readonly IMaintenanceService _maintenanceService;

        public MaintenanceController(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMaintenance([FromQuery] int? assetId, [FromQuery] string? status)
        {
            var records = await _maintenanceService.GetAllMaintenanceRecordsAsync(assetId, status);
            return Ok(records);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMaintenanceById(int id)
        {
            var record = await _maintenanceService.GetMaintenanceByIdAsync(id);
            if (record == null) return NotFound(new { message = $"Maintenance record ID {id} not found." });
            return Ok(record);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMaintenance([FromBody] CreateMaintenanceDto dto)
        {
            var performedBy = GetUserDisplayName();
            var record = await _maintenanceService.CreateMaintenanceAsync(dto, performedBy);
            return CreatedAtAction(nameof(GetMaintenanceById), new { id = record.MaintenanceId }, record);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteMaintenance(int id, [FromBody] CompleteMaintenanceDto dto)
        {
            var performedBy = GetUserDisplayName();
            var record = await _maintenanceService.CompleteMaintenanceAsync(id, dto, performedBy);
            return Ok(record);
        }

        private string GetUserDisplayName()
        {
            var nameClaim = User.FindFirst(ClaimTypes.Name);
            if (nameClaim != null && !string.IsNullOrWhiteSpace(nameClaim.Value)) return nameClaim.Value;
            var emailClaim = User.FindFirst(ClaimTypes.Email);
            return emailClaim?.Value ?? "User";
        }
    }
}
