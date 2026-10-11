using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly ApplicationDbContext _context;

        public MaintenanceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MaintenanceDto>> GetAllMaintenanceRecordsAsync(int? assetId = null, string? status = null)
        {
            var query = _context.MaintenanceRecords
                .Include(m => m.Asset)
                .AsQueryable();

            if (assetId.HasValue && assetId.Value > 0)
            {
                query = query.Where(m => m.AssetId == assetId.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(m => m.Status.ToLower() == status.Trim().ToLower());
            }

            var records = await query.OrderByDescending(m => m.ReportedDate).ToListAsync();

            return records.Select(MapToDto);
        }

        public async Task<MaintenanceDto?> GetMaintenanceByIdAsync(int id)
        {
            var record = await _context.MaintenanceRecords
                .Include(m => m.Asset)
                .FirstOrDefaultAsync(m => m.MaintenanceId == id);

            if (record == null) return null;

            return MapToDto(record);
        }

        public async Task<MaintenanceDto> CreateMaintenanceAsync(CreateMaintenanceDto dto, string performedBy)
        {
            var asset = await _context.Assets.FindAsync(dto.AssetId);
            if (asset == null)
            {
                throw new KeyNotFoundException($"Asset with ID {dto.AssetId} not found.");
            }

            // Update Asset status to UnderMaintenance
            asset.Status = "UnderMaintenance";

            var maintenance = new MaintenanceRecord
            {
                AssetId = dto.AssetId,
                Issue = dto.Issue,
                Vendor = dto.Vendor,
                Cost = dto.Cost,
                ReportedDate = DateTime.UtcNow,
                MaintenanceDate = DateTime.UtcNow,
                Status = "InProgress"
            };

            _context.MaintenanceRecords.Add(maintenance);

            // Log Asset History
            _context.AssetHistories.Add(new AssetHistory
            {
                AssetId = dto.AssetId,
                Action = "MaintenanceStart",
                PerformedBy = performedBy,
                Date = DateTime.UtcNow,
                Description = $"Asset placed under maintenance. Issue: {dto.Issue}. Vendor: {dto.Vendor}"
            });

            await _context.SaveChangesAsync();
            return (await GetMaintenanceByIdAsync(maintenance.MaintenanceId))!;
        }

        public async Task<MaintenanceDto> CompleteMaintenanceAsync(int id, CompleteMaintenanceDto dto, string performedBy)
        {
            var maintenance = await _context.MaintenanceRecords
                .Include(m => m.Asset)
                .FirstOrDefaultAsync(m => m.MaintenanceId == id);

            if (maintenance == null)
            {
                throw new KeyNotFoundException($"Maintenance record ID {id} not found.");
            }

            maintenance.Status = "Completed";
            maintenance.CompletedDate = DateTime.UtcNow;
            maintenance.Resolution = dto.Resolution;

            if (dto.Cost > 0) maintenance.Cost = dto.Cost;
            if (!string.IsNullOrWhiteSpace(dto.Vendor)) maintenance.Vendor = dto.Vendor;

            // Asset status automatically returns to Available
            if (maintenance.Asset != null)
            {
                maintenance.Asset.Status = "Available";
            }

            // Log Asset History
            _context.AssetHistories.Add(new AssetHistory
            {
                AssetId = maintenance.AssetId,
                Action = "MaintenanceCompleted",
                PerformedBy = performedBy,
                Date = DateTime.UtcNow,
                Description = $"Maintenance completed. Resolution: {dto.Resolution}. Cost: ${maintenance.Cost:F2}"
            });

            await _context.SaveChangesAsync();
            return MapToDto(maintenance);
        }

        private static MaintenanceDto MapToDto(MaintenanceRecord m)
        {
            return new MaintenanceDto
            {
                MaintenanceId = m.MaintenanceId,
                AssetId = m.AssetId,
                AssetTag = m.Asset?.AssetTag ?? "",
                AssetName = m.Asset?.AssetName ?? "",
                Issue = m.Issue,
                ReportedDate = m.ReportedDate,
                MaintenanceDate = m.MaintenanceDate,
                Vendor = m.Vendor,
                Cost = m.Cost,
                Resolution = m.Resolution,
                Status = m.Status,
                CompletedDate = m.CompletedDate
            };
        }
    }
}
