using Backend.Data;
using Backend.DTOs;
using Backend.Helpers;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(ApplicationDbContext context, IJwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == loginDto.Email.ToLower() && u.IsActive);

            if (user == null || !PasswordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return null;
            }

            var token = _jwtTokenGenerator.GenerateToken(user, user.Employee);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role,
                EmployeeId = user.EmployeeId,
                Name = user.Employee?.Name ?? "Administrator",
                EmployeeCode = user.Employee?.EmployeeCode ?? "ADMIN",
                Department = user.Employee?.Department ?? "IT Administration"
            };
        }

        public async Task<AuthResponseDto?> GetCurrentUserAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

            if (user == null) return null;

            var token = _jwtTokenGenerator.GenerateToken(user, user.Employee);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role,
                EmployeeId = user.EmployeeId,
                Name = user.Employee?.Name ?? "Administrator",
                EmployeeCode = user.Employee?.EmployeeCode ?? "ADMIN",
                Department = user.Employee?.Department ?? "IT Administration"
            };
        }

        public async Task<bool> RegisterAsync(RegisterUserDto registerDto)
        {
            var existingUser = await _context.Users.AnyAsync(u => u.Email.ToLower() == registerDto.Email.ToLower());
            if (existingUser)
            {
                throw new InvalidOperationException($"User with email '{registerDto.Email}' already exists.");
            }

            var user = new User
            {
                Email = registerDto.Email,
                PasswordHash = PasswordHasher.HashPassword(registerDto.Password),
                Role = registerDto.Role,
                EmployeeId = registerDto.EmployeeId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
