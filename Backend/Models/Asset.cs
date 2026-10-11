using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class Asset
    {
        [Key]
        public int AssetId { get; set; }

        [Required]
        [MaxLength(50)]
        public string AssetTag { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string AssetName { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public AssetCategory? Category { get; set; }

        [MaxLength(100)]
        public string Brand { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Model { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string SerialNumber { get; set; } = string.Empty;

        public DateTime PurchaseDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PurchaseCost { get; set; }

        public DateTime? WarrantyExpiry { get; set; }

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "Available"; // Available, Assigned, UnderMaintenance, Retired

        [MaxLength(150)]
        public string Location { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<AssetAssignment> Assignments { get; set; } = new List<AssetAssignment>();
        public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();
        public ICollection<AssetHistory> Histories { get; set; } = new List<AssetHistory>();
    }
}
