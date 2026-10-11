using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AssignmentsController : ControllerBase
    {
        private readonly IAssignmentService _assignmentService;

        public AssignmentsController(IAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAssignments([FromQuery] int? employeeId, [FromQuery] string? status)
        {
            var assignments = await _assignmentService.GetAllAssignmentsAsync(employeeId, status);
            return Ok(assignments);
        }
    }
}
