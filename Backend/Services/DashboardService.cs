using Backend.Data;
using Backend.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAssetService _assetService;
        private readonly IAssetRequestService _requestService;
        private readonly IAssetHistoryService _historyService;

        public DashboardService(
            ApplicationDbContext context,
            IAssetService assetService,
            IAssetRequestService requestService,
            IAssetHistoryService historyService)
        {
            _context = context;
            _assetService = assetService;
            _requestService = requestService;
            _historyService = historyService;
        }

        public async Task<AdminDashboardDto> GetAdminDashboardMetricsAsync()
        {
            var totalAssets = await _context.Assets.CountAsync();
            var availableAssets = await _context.Assets.CountAsync(a => a.Status == "Available");
            var assignedAssets = await _context.Assets.CountAsync(a => a.Status == "Assigned");
            var underMaintenanceAssets = await _context.Assets.CountAsync(a => a.Status == "UnderMaintenance");
            var retiredAssets = await _context.Assets.CountAsync(a => a.Status == "Retired");
            var pendingRequests = await _context.AssetRequests.CountAsync(r => r.Status == "Pending");
            var totalEmployees = await _context.Employees.CountAsync(e => e.Status == "Active");

            var recentAssets = (await _assetService.GetAssetsAsync()).Take(5).ToList();
            var recentRequests = (await _requestService.GetAllRequestsAsync()).Take(5).ToList();
            var recentActivities = (await _historyService.GetAllHistoriesAsync()).Take(6).ToList();

            return new AdminDashboardDto
            {
                TotalAssets = totalAssets,
                AvailableAssets = availableAssets,
                AssignedAssets = assignedAssets,
                UnderMaintenanceAssets = underMaintenanceAssets,
                RetiredAssets = retiredAssets,
                PendingRequests = pendingRequests,
                TotalEmployees = totalEmployees,
                RecentAssets = recentAssets,
                RecentRequests = recentRequests,
                RecentActivities = recentActivities
            };
        }

        public async Task<EmployeeDashboardDto> GetEmployeeDashboardMetricsAsync(int employeeId)
        {
            var assignedAssets = await _context.AssetAssignments
                .Include(a => a.Asset)
                    .ThenInclude(ast => ast!.Category)
                .Where(a => a.EmployeeId == employeeId && a.Status == "Active")
                .ToListAsync();

            var myRequests = await _requestService.GetAllRequestsAsync(employeeId);

            var myAssignedDtos = assignedAssets.Select(a => new AssetDto
            {
                AssetId = a.Asset!.AssetId,
                AssetTag = a.Asset.AssetTag,
                AssetName = a.Asset.AssetName,
                CategoryId = a.Asset.CategoryId,
                CategoryName = a.Asset.Category?.Name ?? "",
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
            }).ToList();

            var requestsList = myRequests.ToList();

            return new EmployeeDashboardDto
            {
                MyAssignedAssetsCount = myAssignedDtos.Count,
                PendingRequestsCount = requestsList.Count(r => r.Status == "Pending"),
                ApprovedRequestsCount = requestsList.Count(r => r.Status == "Approved" || r.Status == "Completed"),
                UnderMaintenanceCount = myAssignedDtos.Count(a => a.Status == "UnderMaintenance"),
                MyAssignedAssets = myAssignedDtos,
                MyRequests = requestsList
            };
        }
    }
}
