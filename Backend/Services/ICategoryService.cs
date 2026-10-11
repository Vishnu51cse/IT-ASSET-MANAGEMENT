using Backend.DTOs;

namespace Backend.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(int id);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createDto);
        Task<CategoryDto> UpdateCategoryAsync(int id, CreateCategoryDto updateDto);
        Task<bool> DeactivateCategoryAsync(int id);
    }
}
