namespace Backend.DTOs
{
    public class AdminDashboardDto
    {
        public int TotalAssets { get; set; }
        public int AvailableAssets { get; set; }
        public int AssignedAssets { get; set; }
        public int UnderMaintenanceAssets { get; set; }
        public int RetiredAssets { get; set; }
        public int PendingRequests { get; set; }
        public int TotalEmployees { get; set; }
        public List<AssetDto> RecentAssets { get; set; } = new List<AssetDto>();
        public List<AssetRequestDto> RecentRequests { get; set; } = new List<AssetRequestDto>();
        public List<AssetHistoryDto> RecentActivities { get; set; } = new List<AssetHistoryDto>();
    }

    public class EmployeeDashboardDto
    {
        public int MyAssignedAssetsCount { get; set; }
        public int PendingRequestsCount { get; set; }
        public int ApprovedRequestsCount { get; set; }
        public int UnderMaintenanceCount { get; set; }
        public List<AssetDto> MyAssignedAssets { get; set; } = new List<AssetDto>();
        public List<AssetRequestDto> MyRequests { get; set; } = new List<AssetRequestDto>();
    }
}
