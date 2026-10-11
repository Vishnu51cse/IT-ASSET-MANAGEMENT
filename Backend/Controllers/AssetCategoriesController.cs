using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/categories")]
    [Authorize]
    public class AssetCategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public AssetCategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound(new { message = $"Category with ID {id} not found." });
            return Ok(category);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createDto)
        {
            var category = await _categoryService.CreateCategoryAsync(createDto);
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CreateCategoryDto updateDto)
        {
            var category = await _categoryService.UpdateCategoryAsync(id, updateDto);
            return Ok(category);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeactivateCategory(int id)
        {
            var result = await _categoryService.DeactivateCategoryAsync(id);
            if (!result) return NotFound(new { message = $"Category with ID {id} not found." });
            return Ok(new { message = "Category status toggled successfully." });
        }
    }
}
