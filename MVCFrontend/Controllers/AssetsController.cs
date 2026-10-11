using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCFrontend.Models;

namespace MVCFrontend.Controllers
{
    [Authorize]
    public class AssetsController : Controller
    {
        private readonly IAssetService _assetService;
        private readonly ICategoryService _categoryService;
        private readonly IAssetHistoryService _historyService;

        public AssetsController(
            IAssetService assetService,
            ICategoryService categoryService,
            IAssetHistoryService historyService)
        {
            _assetService = assetService;
            _categoryService = categoryService;
            _historyService = historyService;
        }

        public async Task<IActionResult> Index(string search, int? categoryId, string status)
        {
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            var assets = await _assetService.GetAssetsAsync(search, categoryId, status);
            return View(assets);
        }

        public async Task<IActionResult> Details(int id)
        {
            var asset = await _assetService.GetAssetByIdAsync(id);
            if (asset == null) return NotFound();

            var history = await _historyService.GetHistoryByAssetIdAsync(id);
            ViewBag.History = history;

            return View(asset);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = (await _categoryService.GetAllCategoriesAsync()).ToList();
            var model = new MvcCreateAssetViewModel
            {
                AssetTag = $"AST-{Random.Shared.Next(1000, 9999)}",
                SerialNumber = $"SN-{Random.Shared.Next(100000, 999999)}",
                AvailableCategories = categories,
                CategoryId = categories.FirstOrDefault()?.Id ?? 0
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MvcCreateAssetViewModel assetViewModel)
        {
            if (!ModelState.IsValid)
            {
                assetViewModel.AvailableCategories = (await _categoryService.GetAllCategoriesAsync()).ToList();
                return View(assetViewModel);
            }

            try
            {
                var performedBy = User.Identity?.Name ?? "MVC Admin";
                await _assetService.CreateAssetAsync(new CreateAssetDto
                {
                    AssetTag = assetViewModel.AssetTag,
                    AssetName = assetViewModel.AssetName,
                    CategoryId = assetViewModel.CategoryId,
                    Brand = assetViewModel.Brand,
                    Model = assetViewModel.Model,
                    SerialNumber = assetViewModel.SerialNumber,
                    PurchaseDate = assetViewModel.PurchaseDate,
                    PurchaseCost = assetViewModel.PurchaseCost,
                    WarrantyExpiry = assetViewModel.WarrantyExpiry,
                    Location = assetViewModel.Location,
                    Description = assetViewModel.Description
                }, performedBy);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                assetViewModel.AvailableCategories = (await _categoryService.GetAllCategoriesAsync()).ToList();
                return View(assetViewModel);
            }
        }
    }
}
