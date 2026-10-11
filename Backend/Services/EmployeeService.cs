using Backend.Data;
using Backend.DTOs;
using Backend.Helpers;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync(string? search = null)
        {
            var query = _context.Employees
                .Include(e => e.Assignments)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(e =>
                    e.Name.ToLower().Contains(term) ||
                    e.EmployeeCode.ToLower().Contains(term) ||
                    e.Email.ToLower().Contains(term) ||
                    e.Department.ToLower().Contains(term) ||
                    e.Designation.ToLower().Contains(term));
            }

            var employees = await query.OrderByDescending(e => e.CreatedDate).ToListAsync();

            return employees.Select(e => new EmployeeDto
            {
                Id = e.Id,
                EmployeeCode = e.EmployeeCode,
                Name = e.Name,
                Email = e.Email,
                Department = e.Department,
                Designation = e.Designation,
                Phone = e.Phone,
                Status = e.Status,
                CreatedDate = e.CreatedDate,
                ActiveAssignedAssetsCount = e.Assignments.Count(a => a.Status == "Active")
            });
        }

        public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
        {
            var e = await _context.Employees
                .Include(emp => emp.Assignments)
                .FirstOrDefaultAsync(emp => emp.Id == id);

            if (e == null) return null;

            return new EmployeeDto
            {
                Id = e.Id,
                EmployeeCode = e.EmployeeCode,
                Name = e.Name,
                Email = e.Email,
                Department = e.Department,
                Designation = e.Designation,
                Phone = e.Phone,
                Status = e.Status,
                CreatedDate = e.CreatedDate,
                ActiveAssignedAssetsCount = e.Assignments.Count(a => a.Status == "Active")
            };
        }

        public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto createDto)
        {
            if (await _context.Employees.AnyAsync(e => e.EmployeeCode.ToLower() == createDto.EmployeeCode.ToLower()))
            {
                throw new InvalidOperationException($"Employee code '{createDto.EmployeeCode}' already exists.");
            }

            if (await _context.Employees.AnyAsync(e => e.Email.ToLower() == createDto.Email.ToLower()))
            {
                throw new InvalidOperationException($"Email '{createDto.Email}' already exists.");
            }

            var employee = new Employee
            {
                EmployeeCode = createDto.EmployeeCode,
                Name = createDto.Name,
                Email = createDto.Email,
                Department = createDto.Department,
                Designation = createDto.Designation,
                Phone = createDto.Phone,
                Status = "Active",
                CreatedDate = DateTime.UtcNow
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // Create corresponding login account for Employee
            var user = new User
            {
                Email = createDto.Email,
                PasswordHash = PasswordHasher.HashPassword(string.IsNullOrWhiteSpace(createDto.Password) ? "Password123!" : createDto.Password),
                Role = "Employee",
                EmployeeId = employee.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return (await GetEmployeeByIdAsync(employee.Id))!;
        }

        public async Task<EmployeeDto> UpdateEmployeeAsync(int id, UpdateEmployeeDto updateDto)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with ID {id} not found.");
            }

            if (await _context.Employees.AnyAsync(e => e.Id != id && e.Email.ToLower() == updateDto.Email.ToLower()))
            {
                throw new InvalidOperationException($"Email '{updateDto.Email}' is already used by another employee.");
            }

            employee.Name = updateDto.Name;
            employee.Email = updateDto.Email;
            employee.Department = updateDto.Department;
            employee.Designation = updateDto.Designation;
            employee.Phone = updateDto.Phone;
            employee.Status = updateDto.Status;

            // Sync User email and active status if present
            var user = await _context.Users.FirstOrDefaultAsync(u => u.EmployeeId == id);
            if (user != null)
            {
                user.Email = updateDto.Email;
                user.IsActive = (updateDto.Status == "Active");
            }

            await _context.SaveChangesAsync();
            return (await GetEmployeeByIdAsync(id))!;
        }

        public async Task<bool> DeactivateEmployeeAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return false;

            employee.Status = "Deactivated";

            var user = await _context.Users.FirstOrDefaultAsync(u => u.EmployeeId == id);
            if (user != null)
            {
                user.IsActive = false;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AssetDto>> GetAssignedAssetsForEmployeeAsync(int employeeId)
        {
            var activeAssignments = await _context.AssetAssignments
                .Include(a => a.Asset)
                    .ThenInclude(ast => ast!.Category)
                .Where(a => a.EmployeeId == employeeId && a.Status == "Active")
                .ToListAsync();

            return activeAssignments.Select(a => new AssetDto
            {
                AssetId = a.Asset!.AssetId,
                AssetTag = a.Asset.AssetTag,
                AssetName = a.Asset.AssetName,
                CategoryId = a.Asset.CategoryId,
                CategoryName = a.Asset.Category?.Name ?? "General",
                Brand = a.Asset.Brand,
                Model = a.Asset.Model,
                SerialNumber = a.Asset.SerialNumber,
                PurchaseDate = a.Asset.PurchaseDate,
                PurchaseCost = a.Asset.PurchaseCost,
                WarrantyExpiry = a.Asset.WarrantyExpiry,
                Status = a.Asset.Status,
                Location = a.Asset.Location,
                Description = a.Asset.Description,
                CreatedDate = a.Asset.CreatedDate,
                CurrentEmployeeId = employeeId,
                CurrentEmployeeName = a.Employee?.Name ?? ""
            });
        }
    }
}
