using System.ComponentModel.DataAnnotations;
using Backend.DTOs;

namespace MVCFrontend.Models
{
    public class MvcLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "admin@company.com";

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "Password123!";
    }

    public class MvcCreateAssetViewModel
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
        public DateTime PurchaseDate { get; set; } = DateTime.Today;

        [Range(0, 1000000)]
        public decimal PurchaseCost { get; set; } = 999.00m;

        public DateTime? WarrantyExpiry { get; set; }
        public string Location { get; set; } = "Building A";
        public string Description { get; set; } = string.Empty;

        public List<CategoryDto> AvailableCategories { get; set; } = new List<CategoryDto>();
    }

    public class MvcAssignAssetViewModel
    {
        [Required]
        public int AssetId { get; set; }
        public string AssetName { get; set; } = string.Empty;
        public string AssetTag { get; set; } = string.Empty;

        [Required]
        public int EmployeeId { get; set; }

        public DateTime? ExpectedReturnDate { get; set; }
        public string Notes { get; set; } = string.Empty;

        public List<EmployeeDto> AvailableEmployees { get; set; } = new List<EmployeeDto>();
    }

    public class MvcReturnAssetViewModel
    {
        [Required]
        public int AssetId { get; set; }
        public string AssetName { get; set; } = string.Empty;
        public string AssetTag { get; set; } = string.Empty;
        public string AssignedToEmployee { get; set; } = string.Empty;

        public string Condition { get; set; } = "Good";
        public string ReturnNotes { get; set; } = string.Empty;
    }
}
