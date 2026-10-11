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
    public class AssetsController : ControllerBase
    {
        private readonly IAssetService _assetService;
        private readonly IAssignmentService _assignmentService;
        private readonly IMaintenanceService _maintenanceService;

        public AssetsController(
            IAssetService assetService,
            IAssignmentService assignmentService,
            IMaintenanceService maintenanceService)
        {
            _assetService = assetService;
            _assignmentService = assignmentService;
            _maintenanceService = maintenanceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAssets(
            [FromQuery] string? search,
            [FromQuery] int? categoryId,
            [FromQuery] string? status)
        {
            var assets = await _assetService.GetAssetsAsync(search, categoryId, status);
            return Ok(assets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAssetById(int id)
        {
            var asset = await _assetService.GetAssetByIdAsync(id);
            if (asset == null) return NotFound(new { message = $"Asset with ID {id} not found." });
            return Ok(asset);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateAsset([FromBody] CreateAssetDto createDto)
        {
            var performedBy = GetUserDisplayName();
            var asset = await _assetService.CreateAssetAsync(createDto, performedBy);
            return CreatedAtAction(nameof(GetAssetById), new { id = asset.AssetId }, asset);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("procure-and-assign")]
        public async Task<IActionResult> ProcureAndAssignAsset([FromBody] ProcureAndAssignAssetDto dto)
        {
            var performedBy = GetUserDisplayName();
            var asset = await _assetService.ProcureAndAssignAssetAsync(dto, performedBy);
            return CreatedAtAction(nameof(GetAssetById), new { id = asset.AssetId }, asset);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsset(int id, [FromBody] UpdateAssetDto updateDto)
        {
            var performedBy = GetUserDisplayName();
            var asset = await _assetService.UpdateAssetAsync(id, updateDto, performedBy);
            return Ok(asset);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsset(int id)
        {
            var performedBy = GetUserDisplayName();
            var result = await _assetService.DeleteAssetAsync(id, performedBy);
            if (!result) return NotFound(new { message = $"Asset with ID {id} not found." });
            return Ok(new { message = "Asset retired successfully." });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/assign")]
        public async Task<IActionResult> AssignAsset(int id, [FromBody] AssignAssetDto dto)
        {
            dto.AssetId = id;
            var performedBy = GetUserDisplayName();
            var assignment = await _assignmentService.AssignAssetAsync(dto, performedBy);
            return Ok(assignment);
        }

        [HttpPost("{id}/return")]
        public async Task<IActionResult> ReturnAsset(int id, [FromBody] ReturnAssetDto dto)
        {
            dto.AssetId = id;
            var performedBy = GetUserDisplayName();
            var assignment = await _assignmentService.ReturnAssetAsync(dto, performedBy);
            return Ok(assignment);
        }

        [HttpPost("{id}/maintenance")]
        public async Task<IActionResult> PutUnderMaintenance(int id, [FromBody] CreateMaintenanceDto dto)
        {
            dto.AssetId = id;
            var performedBy = GetUserDisplayName();
            var record = await _maintenanceService.CreateMaintenanceAsync(dto, performedBy);
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
