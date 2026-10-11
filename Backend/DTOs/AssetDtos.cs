using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public class AssetDto
    {
        public int AssetId { get; set; }
        public string AssetTag { get; set; } = string.Empty;
        public string AssetName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public decimal PurchaseCost { get; set; }
        public DateTime? WarrantyExpiry { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public int? CurrentEmployeeId { get; set; }
        public string CurrentEmployeeName { get; set; } = string.Empty;
    }

    public class CreateAssetDto
    {
        [Required]
        public string AssetTag { get; set; } = string.Empty;

        [Required]
        public string AssetName { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;

        [Required]
        public string SerialNumber { get; set; } = string.Empty;

        [Required]
        public DateTime PurchaseDate { get; set; }

        [Range(0, 1000000)]
        public decimal PurchaseCost { get; set; }

        public DateTime? WarrantyExpiry { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateAssetDto
    {
        [Required]
        public string AssetName { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;

        [Range(0, 1000000)]
        public decimal PurchaseCost { get; set; }

        public DateTime? WarrantyExpiry { get; set; }
        public string Status { get; set; } = "Available";
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class ProcureAndAssignAssetDto : CreateAssetDto
    {
        [Required]
        public int EmployeeId { get; set; }
        public DateTime? ExpectedReturnDate { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
