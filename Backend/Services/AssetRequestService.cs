using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class AssetRequestService : IAssetRequestService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAssignmentService _assignmentService;

        public AssetRequestService(ApplicationDbContext context, IAssignmentService assignmentService)
        {
            _context = context;
            _assignmentService = assignmentService;
        }

        public async Task<IEnumerable<AssetRequestDto>> GetAllRequestsAsync(int? employeeId = null, string? status = null)
        {
            var query = _context.AssetRequests
                .Include(r => r.Employee)
                .Include(r => r.Category)
                .AsQueryable();

            if (employeeId.HasValue && employeeId.Value > 0)
            {
                query = query.Where(r => r.EmployeeId == employeeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(r => r.Status.ToLower() == status.Trim().ToLower());
            }

            var requests = await query.OrderByDescending(r => r.RequestedDate).ToListAsync();

            return requests.Select(MapToDto);
        }

        public async Task<AssetRequestDto?> GetRequestByIdAsync(int id)
        {
            var request = await _context.AssetRequests
                .Include(r => r.Employee)
                .Include(r => r.Category)
                .FirstOrDefaultAsync(r => r.RequestId == id);

            if (request == null) return null;

            return MapToDto(request);
        }

        public async Task<AssetRequestDto> CreateRequestAsync(int employeeId, CreateAssetRequestDto dto)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee with ID {employeeId} not found.");
            }

            var category = await _context.Categories.FindAsync(dto.CategoryId);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {dto.CategoryId} not found.");
            }

            var request = new AssetRequest
            {
                EmployeeId = employeeId,
                CategoryId = dto.CategoryId,
                Reason = dto.Reason,
                RequestedDate = DateTime.UtcNow,
                Status = "Pending",
                AdminComment = string.Empty
            };

            _context.AssetRequests.Add(request);
            await _context.SaveChangesAsync();

            return (await GetRequestByIdAsync(request.RequestId))!;
        }

        public async Task<AssetRequestDto> ProcessRequestAsync(int requestId, ProcessRequestDto dto, string adminName)
        {
            var request = await _context.AssetRequests
                .Include(r => r.Employee)
                .Include(r => r.Category)
                .FirstOrDefaultAsync(r => r.RequestId == requestId);

            if (request == null)
            {
                throw new KeyNotFoundException($"Asset request ID {requestId} not found.");
            }

            if (dto.Action.Equals("Approve", StringComparison.OrdinalIgnoreCase))
            {
                request.Status = "Approved";
                request.AdminComment = dto.AdminComment;

                if (dto.AssignAssetId.HasValue && dto.AssignAssetId.Value > 0)
                {
                    // Directly assign the selected asset to the employee
                    await _assignmentService.AssignAssetAsync(new AssignAssetDto
                    {
                        AssetId = dto.AssignAssetId.Value,
                        EmployeeId = request.EmployeeId,
                        Notes = $"Assigned upon approving Request #{request.RequestId}. Reason: {request.Reason}"
                    }, adminName);

                    request.Status = "Completed";
                }
            }
            else if (dto.Action.Equals("Reject", StringComparison.OrdinalIgnoreCase))
            {
                request.Status = "Rejected";
                request.AdminComment = dto.AdminComment;
            }
            else
            {
                throw new ArgumentException("Action must be either 'Approve' or 'Reject'.");
            }

            await _context.SaveChangesAsync();
            return MapToDto(request);
        }

        private static AssetRequestDto MapToDto(AssetRequest r)
        {
            return new AssetRequestDto
            {
                RequestId = r.RequestId,
                EmployeeId = r.EmployeeId,
                EmployeeName = r.Employee?.Name ?? "",
                EmployeeCode = r.Employee?.EmployeeCode ?? "",
                Department = r.Employee?.Department ?? "",
                CategoryId = r.CategoryId,
                CategoryName = r.Category?.Name ?? "",
                Reason = r.Reason,
                RequestedDate = r.RequestedDate,
                Status = r.Status,
                AdminComment = r.AdminComment
            };
        }
    }
}
