using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public class CreateMaintenanceDto
    {
        [Required]
        public int AssetId { get; set; }

        [Required]
        public string Issue { get; set; } = string.Empty;

        public string Vendor { get; set; } = string.Empty;
        public decimal Cost { get; set; } = 0;
    }

    public class CompleteMaintenanceDto
    {
        public string Resolution { get; set; } = string.Empty;
        public decimal Cost { get; set; } = 0;
        public string Vendor { get; set; } = string.Empty;
    }

    public class MaintenanceDto
    {
        public int MaintenanceId { get; set; }
        public int AssetId { get; set; }
        public string AssetTag { get; set; } = string.Empty;
        public string AssetName { get; set; } = string.Empty;
        public string Issue { get; set; } = string.Empty;
        public DateTime ReportedDate { get; set; }
        public DateTime? MaintenanceDate { get; set; }
        public string Vendor { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public string Resolution { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? CompletedDate { get; set; }
    }
}
