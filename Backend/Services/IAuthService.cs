using Backend.DTOs;

namespace Backend.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto?> GetCurrentUserAsync(int userId);
        Task<bool> RegisterAsync(RegisterUserDto registerDto);
    }
}
