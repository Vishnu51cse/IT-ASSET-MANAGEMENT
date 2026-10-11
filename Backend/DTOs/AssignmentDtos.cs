using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public class AssignAssetDto
    {
        [Required]
        public int AssetId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        public DateTime? ExpectedReturnDate { get; set; }
        public string Notes { get; set; } = string.Empty;
    }

    public class ReturnAssetDto
    {
        [Required]
        public int AssetId { get; set; }

        public DateTime ReturnDate { get; set; } = DateTime.UtcNow;
        public string Condition { get; set; } = "Good";
        public string ReturnNotes { get; set; } = string.Empty;
    }

    public class AssignmentDto
    {
        public int Id { get; set; }
        public int AssetId { get; set; }
        public string AssetTag { get; set; } = string.Empty;
        public string AssetName { get; set; } = string.Empty;
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public DateTime AssignedDate { get; set; }
        public DateTime? ExpectedReturnDate { get; set; }
        public DateTime? ActualReturnDate { get; set; }
        public string AssignedBy { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
