using System.Security.Claims;
using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/asset-requests")]
    [Authorize]
    public class AssetRequestsController : ControllerBase
    {
        private readonly IAssetRequestService _requestService;

        public AssetRequestsController(IAssetRequestService requestService)
        {
            _requestService = requestService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRequests([FromQuery] int? employeeId, [FromQuery] string? status)
        {
            var requests = await _requestService.GetAllRequestsAsync(employeeId, status);
            return Ok(requests);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRequestById(int id)
        {
            var request = await _requestService.GetRequestByIdAsync(id);
            if (request == null) return NotFound(new { message = $"Request ID {id} not found." });
            return Ok(request);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest([FromBody] CreateAssetRequestDto dto)
        {
            var empClaim = User.FindFirst("EmployeeId");
            if (empClaim == null || !int.TryParse(empClaim.Value, out int employeeId))
            {
                return BadRequest(new { message = "Only registered employees can submit asset requests." });
            }

            var request = await _requestService.CreateRequestAsync(employeeId, dto);
            return CreatedAtAction(nameof(GetRequestById), new { id = request.RequestId }, request);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveRequest(int id, [FromBody] ProcessRequestDto dto)
        {
            dto.Action = "Approve";
            var adminName = GetUserDisplayName();
            var result = await _requestService.ProcessRequestAsync(id, dto, adminName);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectRequest(int id, [FromBody] ProcessRequestDto dto)
        {
            dto.Action = "Reject";
            var adminName = GetUserDisplayName();
            var result = await _requestService.ProcessRequestAsync(id, dto, adminName);
            return Ok(result);
        }

        private string GetUserDisplayName()
        {
            var nameClaim = User.FindFirst(ClaimTypes.Name);
            if (nameClaim != null && !string.IsNullOrWhiteSpace(nameClaim.Value)) return nameClaim.Value;
            var emailClaim = User.FindFirst(ClaimTypes.Email);
            return emailClaim?.Value ?? "Admin";
        }
    }
}
