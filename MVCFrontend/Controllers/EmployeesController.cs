using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MVCFrontend.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<IActionResult> Index(string search)
        {
            var employees = await _employeeService.GetAllEmployeesAsync(search);
            return View(employees);
        }
    }
}
