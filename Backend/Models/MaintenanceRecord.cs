using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class MaintenanceRecord
    {
        [Key]
        public int MaintenanceId { get; set; }

        [Required]
        public int AssetId { get; set; }

        [ForeignKey(nameof(AssetId))]
        public Asset? Asset { get; set; }

        [Required]
        [MaxLength(500)]
        public string Issue { get; set; } = string.Empty;

        public DateTime ReportedDate { get; set; } = DateTime.UtcNow;

        public DateTime? MaintenanceDate { get; set; }

        [MaxLength(150)]
        public string Vendor { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; } = 0;

        [MaxLength(500)]
        public string Resolution { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "Open"; // Open, InProgress, Completed

        public DateTime? CompletedDate { get; set; }
    }
}
