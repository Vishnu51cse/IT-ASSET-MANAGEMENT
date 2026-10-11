using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly ApplicationDbContext _context;

        public AssignmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AssignmentDto>> GetAllAssignmentsAsync(int? employeeId = null, string? status = null)
        {
            var query = _context.AssetAssignments
                .Include(a => a.Asset)
                .Include(a => a.Employee)
                .AsQueryable();

            if (employeeId.HasValue && employeeId.Value > 0)
            {
                query = query.Where(a => a.EmployeeId == employeeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(a => a.Status.ToLower() == status.Trim().ToLower());
            }

            var assignments = await query.OrderByDescending(a => a.AssignedDate).ToListAsync();

            return assignments.Select(a => new AssignmentDto
            {
                Id = a.Id,
                AssetId = a.AssetId,
                AssetTag = a.Asset?.AssetTag ?? "",
                AssetName = a.Asset?.AssetName ?? "",
                EmployeeId = a.EmployeeId,
                EmployeeName = a.Employee?.Name ?? "",
                EmployeeCode = a.Employee?.EmployeeCode ?? "",
                AssignedDate = a.AssignedDate,
                ExpectedReturnDate = a.ExpectedReturnDate,
                ActualReturnDate = a.ActualReturnDate,
                AssignedBy = a.AssignedBy,
                Notes = a.Notes,
                Status = a.Status
            });
        }

        public async Task<AssignmentDto> AssignAssetAsync(AssignAssetDto dto, string performedBy)
        {
            var asset = await _context.Assets.FindAsync(dto.AssetId);
            if (asset == null)
            {
                throw new KeyNotFoundException($"Asset with ID {dto.AssetId} not found.");
            }

            if (asset.Status != "Available")
            {
                throw new InvalidOperationException($"Asset '{asset.AssetName}' ({asset.AssetTag}) is currently '{asset.Status}' and cannot be assigned.");
            }

            var employee = await _context.Employees.FindAsync(dto.EmployeeId);
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with ID {dto.EmployeeId} not found.");
            }

            if (employee.Status != "Active")
            {
                throw new InvalidOperationException($"Cannot assign asset to deactivated employee '{employee.Name}'.");
            }

            // Update Asset status to Assigned
            asset.Status = "Assigned";

            // Create assignment record
            var assignment = new AssetAssignment
            {
                AssetId = dto.AssetId,
                EmployeeId = dto.EmployeeId,
                AssignedDate = DateTime.UtcNow,
                ExpectedReturnDate = dto.ExpectedReturnDate,
                AssignedBy = performedBy,
                Notes = dto.Notes,
                Status = "Active"
            };

            _context.AssetAssignments.Add(assignment);

            // Log Asset History
            _context.AssetHistories.Add(new AssetHistory
            {
                AssetId = dto.AssetId,
                Action = "Assigned",
                PerformedBy = performedBy,
                Date = DateTime.UtcNow,
                Description = $"Asset assigned to {employee.Name} ({employee.EmployeeCode}). Notes: {dto.Notes}"
            });

            await _context.SaveChangesAsync();

            return new AssignmentDto
            {
                Id = assignment.Id,
                AssetId = asset.AssetId,
                AssetTag = asset.AssetTag,
                AssetName = asset.AssetName,
                EmployeeId = employee.Id,
                EmployeeName = employee.Name,
                EmployeeCode = employee.EmployeeCode,
                AssignedDate = assignment.AssignedDate,
                ExpectedReturnDate = assignment.ExpectedReturnDate,
                AssignedBy = assignment.AssignedBy,
                Notes = assignment.Notes,
                Status = assignment.Status
            };
        }

        public async Task<AssignmentDto> ReturnAssetAsync(ReturnAssetDto dto, string performedBy)
        {
            var asset = await _context.Assets.FindAsync(dto.AssetId);
            if (asset == null)
            {
                throw new KeyNotFoundException($"Asset with ID {dto.AssetId} not found.");
            }

            var assignment = await _context.AssetAssignments
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.AssetId == dto.AssetId && a.Status == "Active");

            if (assignment == null)
            {
                throw new InvalidOperationException($"No active assignment found for asset '{asset.AssetName}' ({asset.AssetTag}).");
            }

            // Update assignment status
            assignment.Status = "Returned";
            assignment.ActualReturnDate = dto.ReturnDate;

            // Update Asset status to Available
            asset.Status = "Available";

            // Log Asset History
            _context.AssetHistories.Add(new AssetHistory
            {
                AssetId = dto.AssetId,
                Action = "Returned",
                PerformedBy = performedBy,
                Date = dto.ReturnDate,
                Description = $"Asset returned by {assignment.Employee?.Name ?? "Employee"}. Condition: {dto.Condition}. Notes: {dto.ReturnNotes}"
            });

            await _context.SaveChangesAsync();

            return new AssignmentDto
            {
                Id = assignment.Id,
                AssetId = asset.AssetId,
                AssetTag = asset.AssetTag,
                AssetName = asset.AssetName,
                EmployeeId = assignment.EmployeeId,
                EmployeeName = assignment.Employee?.Name ?? "",
                EmployeeCode = assignment.Employee?.EmployeeCode ?? "",
                AssignedDate = assignment.AssignedDate,
                ExpectedReturnDate = assignment.ExpectedReturnDate,
                ActualReturnDate = assignment.ActualReturnDate,
                AssignedBy = assignment.AssignedBy,
                Notes = $"Condition: {dto.Condition}. Notes: {dto.ReturnNotes}",
                Status = assignment.Status
            };
        }
    }
}
