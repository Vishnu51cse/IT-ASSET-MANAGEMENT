using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories
                .Include(c => c.Assets)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                IsActive = c.IsActive,
                TotalAssetsCount = c.Assets.Count
            });
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var c = await _context.Categories
                .Include(cat => cat.Assets)
                .FirstOrDefaultAsync(cat => cat.Id == id);

            if (c == null) return null;

            return new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                IsActive = c.IsActive,
                TotalAssetsCount = c.Assets.Count
            };
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createDto)
        {
            if (await _context.Categories.AnyAsync(c => c.Name.ToLower() == createDto.Name.ToLower()))
            {
                throw new InvalidOperationException($"Category with name '{createDto.Name}' already exists.");
            }

            var category = new AssetCategory
            {
                Name = createDto.Name,
                Description = createDto.Description,
                IsActive = true
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return (await GetCategoryByIdAsync(category.Id))!;
        }

        public async Task<CategoryDto> UpdateCategoryAsync(int id, CreateCategoryDto updateDto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }

            if (await _context.Categories.AnyAsync(c => c.Id != id && c.Name.ToLower() == updateDto.Name.ToLower()))
            {
                throw new InvalidOperationException($"Category with name '{updateDto.Name}' already exists.");
            }

            category.Name = updateDto.Name;
            category.Description = updateDto.Description;

            await _context.SaveChangesAsync();
            return (await GetCategoryByIdAsync(id))!;
        }

        public async Task<bool> DeactivateCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            category.IsActive = !category.IsActive; // Toggle active status
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
