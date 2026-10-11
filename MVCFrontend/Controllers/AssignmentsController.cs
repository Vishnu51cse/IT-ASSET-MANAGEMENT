using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCFrontend.Models;

namespace MVCFrontend.Controllers
{
    [Authorize]
    public class AssignmentsController : Controller
    {
        private readonly IAssetService _assetService;
        private readonly IEmployeeService _employeeService;
        private readonly IAssignmentService _assignmentService;

        public AssignmentsController(
            IAssetService assetService,
            IEmployeeService employeeService,
            IAssignmentService assignmentService)
        {
            _assetService = assetService;
            _employeeService = employeeService;
            _assignmentService = assignmentService;
        }

        [HttpGet]
        public async Task<IActionResult> Assign(int assetId)
        {
            var asset = await _assetService.GetAssetByIdAsync(assetId);
            if (asset == null) return NotFound();

            var employees = (await _employeeService.GetAllEmployeesAsync()).Where(e => e.Status == "Active").ToList();

            var model = new MvcAssignAssetViewModel
            {
                AssetId = asset.AssetId,
                AssetName = asset.AssetName,
                AssetTag = asset.AssetTag,
                AvailableEmployees = employees,
                EmployeeId = employees.FirstOrDefault()?.Id ?? 0
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Assign(MvcAssignAssetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableEmployees = (await _employeeService.GetAllEmployeesAsync()).Where(e => e.Status == "Active").ToList();
                return View(model);
            }

            try
            {
                var performedBy = User.Identity?.Name ?? "MVC Admin";
                await _assignmentService.AssignAssetAsync(new AssignAssetDto
                {
                    AssetId = model.AssetId,
                    EmployeeId = model.EmployeeId,
                    ExpectedReturnDate = model.ExpectedReturnDate,
                    Notes = model.Notes
                }, performedBy);

                return RedirectToAction("Index", "Assets");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                model.AvailableEmployees = (await _employeeService.GetAllEmployeesAsync()).Where(e => e.Status == "Active").ToList();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Return(int assetId)
        {
            var asset = await _assetService.GetAssetByIdAsync(assetId);
            if (asset == null) return NotFound();

            var model = new MvcReturnAssetViewModel
            {
                AssetId = asset.AssetId,
                AssetName = asset.AssetName,
                AssetTag = asset.AssetTag,
                AssignedToEmployee = asset.CurrentEmployeeName
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Return(MvcReturnAssetViewModel model)
        {
            try
            {
                var performedBy = User.Identity?.Name ?? "MVC Admin";
                await _assignmentService.ReturnAssetAsync(new ReturnAssetDto
                {
                    AssetId = model.AssetId,
                    Condition = model.Condition,
                    ReturnNotes = model.ReturnNotes,
                    ReturnDate = DateTime.UtcNow
                }, performedBy);

                return RedirectToAction("Index", "Assets");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }
    }
}
