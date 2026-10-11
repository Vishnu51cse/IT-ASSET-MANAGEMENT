using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees([FromQuery] string? search)
        {
            var employees = await _employeeService.GetAllEmployeesAsync(search);
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null) return NotFound(new { message = $"Employee with ID {id} not found." });
            return Ok(employee);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto createDto)
        {
            var employee = await _employeeService.CreateEmployeeAsync(createDto);
            return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, employee);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto updateDto)
        {
            var employee = await _employeeService.UpdateEmployeeAsync(id, updateDto);
            return Ok(employee);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeactivateEmployee(int id)
        {
            var result = await _employeeService.DeactivateEmployeeAsync(id);
            if (!result) return NotFound(new { message = $"Employee with ID {id} not found." });
            return Ok(new { message = "Employee deactivated successfully." });
        }

        [HttpGet("{id}/assets")]
        public async Task<IActionResult> GetEmployeeAssignedAssets(int id)
        {
            var assets = await _employeeService.GetAssignedAssetsForEmployeeAsync(id);
            return Ok(assets);
        }
    }
}
