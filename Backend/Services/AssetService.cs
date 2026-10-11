using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class AssetService : IAssetService
    {
        private readonly ApplicationDbContext _context;

        public AssetService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AssetDto>> GetAssetsAsync(string? search = null, int? categoryId = null, string? status = null)
        {
            var query = _context.Assets
                .Include(a => a.Category)
                .Include(a => a.Assignments.Where(asgn => asgn.Status == "Active"))
                    .ThenInclude(asgn => asgn.Employee)
                .AsQueryable();

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(a => a.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(a => a.Status.ToLower() == status.Trim().ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(a =>
                    a.AssetTag.ToLower().Contains(term) ||
                    a.AssetName.ToLower().Contains(term) ||
                    a.SerialNumber.ToLower().Contains(term) ||
                    a.Brand.ToLower().Contains(term) ||
                    a.Model.ToLower().Contains(term) ||
                    a.Location.ToLower().Contains(term));
            }

            var assets = await query.OrderByDescending(a => a.CreatedDate).ToListAsync();

            return assets.Select(MapToDto);
        }

        public async Task<AssetDto?> GetAssetByIdAsync(int id)
        {
            var asset = await _context.Assets
                .Include(a => a.Category)
                .Include(a => a.Assignments.Where(asgn => asgn.Status == "Active"))
                    .ThenInclude(asgn => asgn.Employee)
                .FirstOrDefaultAsync(a => a.AssetId == id);

            if (asset == null) return null;

            return MapToDto(asset);
        }

        public async Task<AssetDto> CreateAssetAsync(CreateAssetDto createDto, string performedBy)
        {
            if (await _context.Assets.AnyAsync(a => a.AssetTag.ToLower() == createDto.AssetTag.ToLower()))
            {
                throw new InvalidOperationException($"Asset tag '{createDto.AssetTag}' already exists.");
            }

            if (await _context.Assets.AnyAsync(a => a.SerialNumber.ToLower() == createDto.SerialNumber.ToLower()))
            {
                throw new InvalidOperationException($"Serial number '{createDto.SerialNumber}' already exists.");
            }

            var category = await _context.Categories.FindAsync(createDto.CategoryId);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category ID {createDto.CategoryId} does not exist.");
            }

            var asset = new Asset
            {
                AssetTag = createDto.AssetTag,
                AssetName = createDto.AssetName,
                CategoryId = createDto.CategoryId,
                Brand = createDto.Brand,
                Model = createDto.Model,
                SerialNumber = createDto.SerialNumber,
                PurchaseDate = createDto.PurchaseDate,
                PurchaseCost = createDto.PurchaseCost,
                WarrantyExpiry = createDto.WarrantyExpiry,
                Status = "Available",
                Location = createDto.Location,
                Description = createDto.Description,
                CreatedDate = DateTime.UtcNow
            };

            _context.Assets.Add(asset);
            await _context.SaveChangesAsync();

            // Log Asset Created History
            _context.AssetHistories.Add(new AssetHistory
            {
                AssetId = asset.AssetId,
                Action = "Created",
                PerformedBy = performedBy,
                Date = DateTime.UtcNow,
                Description = $"Asset {asset.AssetTag} ({asset.AssetName}) registered into inventory."
            });
            await _context.SaveChangesAsync();

            return (await GetAssetByIdAsync(asset.AssetId))!;
        }

        public async Task<AssetDto> ProcureAndAssignAssetAsync(ProcureAndAssignAssetDto dto, string performedBy)
        {
            var employee = await _context.Employees.FindAsync(dto.EmployeeId);
            if (employee == null)
            {
                throw new KeyNotFoundException($"Employee ID {dto.EmployeeId} not found.");
            }

            // Create asset directly in Assigned state
            var assetDto = await CreateAssetAsync(dto, performedBy);
            var asset = await _context.Assets.FindAsync(assetDto.AssetId);
            if (asset != null)
            {
                asset.Status = "Assigned";

                var assignment = new AssetAssignment
                {
                    AssetId = asset.AssetId,
                    EmployeeId = dto.EmployeeId,
                    AssignedDate = DateTime.UtcNow,
                    ExpectedReturnDate = dto.ExpectedReturnDate,
                    AssignedBy = performedBy,
                    Notes = string.IsNullOrWhiteSpace(dto.Notes) ? "Newly procured asset assigned upon purchase." : dto.Notes,
                    Status = "Active"
                };

                _context.AssetAssignments.Add(assignment);

                _context.AssetHistories.Add(new AssetHistory
                {
                    AssetId = asset.AssetId,
                    Action = "Assigned",
                    PerformedBy = performedBy,
                    Date = DateTime.UtcNow,
                    Description = $"Newly procured asset assigned directly to {employee.Name} ({employee.EmployeeCode})."
                });

                await _context.SaveChangesAsync();
            }

            return (await GetAssetByIdAsync(assetDto.AssetId))!;
        }

        public async Task<AssetDto> UpdateAssetAsync(int id, UpdateAssetDto updateDto, string performedBy)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset == null)
            {
                throw new KeyNotFoundException($"Asset with ID {id} not found.");
            }

            var category = await _context.Categories.FindAsync(updateDto.CategoryId);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category ID {updateDto.CategoryId} does not exist.");
            }

            var oldStatus = asset.Status;

            asset.AssetName = updateDto.AssetName;
            asset.CategoryId = updateDto.CategoryId;
            asset.Brand = updateDto.Brand;
            asset.Model = updateDto.Model;
            asset.PurchaseCost = updateDto.PurchaseCost;
            asset.WarrantyExpiry = updateDto.WarrantyExpiry;
            asset.Status = updateDto.Status;
            asset.Location = updateDto.Location;
            asset.Description = updateDto.Description;

            await _context.SaveChangesAsync();

            // Log update history
            _context.AssetHistories.Add(new AssetHistory
            {
                AssetId = id,
                Action = "Updated",
                PerformedBy = performedBy,
                Date = DateTime.UtcNow,
                Description = oldStatus != updateDto.Status
                    ? $"Asset details updated. Status changed from {oldStatus} to {updateDto.Status}."
                    : $"Asset details updated."
            });
            await _context.SaveChangesAsync();

            return (await GetAssetByIdAsync(id))!;
        }

        public async Task<bool> DeleteAssetAsync(int id, string performedBy)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset == null) return false;

            // Instead of deleting, mark status as Retired to preserve full audit trail
            asset.Status = "Retired";

            _context.AssetHistories.Add(new AssetHistory
            {
                AssetId = id,
                Action = "Retired",
                PerformedBy = performedBy,
                Date = DateTime.UtcNow,
                Description = $"Asset {asset.AssetTag} has been retired from active service."
            });

            await _context.SaveChangesAsync();
            return true;
        }

        private static AssetDto MapToDto(Asset a)
        {
            var activeAssignment = a.Assignments?.FirstOrDefault(asgn => asgn.Status == "Active");
            return new AssetDto
            {
                AssetId = a.AssetId,
                AssetTag = a.AssetTag,
                AssetName = a.AssetName,
                CategoryId = a.CategoryId,
                CategoryName = a.Category?.Name ?? "General",
                Brand = a.Brand,
                Model = a.Model,
                SerialNumber = a.SerialNumber,
                PurchaseDate = a.PurchaseDate,
                PurchaseCost = a.PurchaseCost,
                WarrantyExpiry = a.WarrantyExpiry,
                Status = a.Status,
                Location = a.Location,
                Description = a.Description,
                CreatedDate = a.CreatedDate,
                CurrentEmployeeId = activeAssignment?.EmployeeId,
                CurrentEmployeeName = activeAssignment?.Employee?.Name ?? string.Empty
            };
        }
    }
}
