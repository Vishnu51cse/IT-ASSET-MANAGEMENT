using Backend.Data;
using Backend.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class AssetHistoryService : IAssetHistoryService
    {
        private readonly ApplicationDbContext _context;

        public AssetHistoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AssetHistoryDto>> GetAllHistoriesAsync()
        {
            var histories = await _context.AssetHistories
                .Include(h => h.Asset)
                .OrderByDescending(h => h.Date)
                .ToListAsync();

            return histories.Select(h => new AssetHistoryDto
            {
                HistoryId = h.HistoryId,
                AssetId = h.AssetId,
                AssetTag = h.Asset?.AssetTag ?? "",
                AssetName = h.Asset?.AssetName ?? "",
                Action = h.Action,
                PerformedBy = h.PerformedBy,
                Date = h.Date,
                Description = h.Description
            });
        }

        public async Task<IEnumerable<AssetHistoryDto>> GetHistoryByAssetIdAsync(int assetId)
        {
            var histories = await _context.AssetHistories
                .Include(h => h.Asset)
                .Where(h => h.AssetId == assetId)
                .OrderByDescending(h => h.Date)
                .ToListAsync();

            return histories.Select(h => new AssetHistoryDto
            {
                HistoryId = h.HistoryId,
                AssetId = h.AssetId,
                AssetTag = h.Asset?.AssetTag ?? "",
                AssetName = h.Asset?.AssetName ?? "",
                Action = h.Action,
                PerformedBy = h.PerformedBy,
                Date = h.Date,
                Description = h.Description
            });
        }
    }
}
