using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public class CreateAssetRequestDto
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;
    }

    public class ProcessRequestDto
    {
        [Required]
        public string Action { get; set; } = string.Empty; // Approve, Reject

        public string AdminComment { get; set; } = string.Empty;

        public int? AssignAssetId { get; set; } // Optional asset to assign directly on approval
    }

    public class AssetRequestDto
    {
        public int RequestId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime RequestedDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string AdminComment { get; set; } = string.Empty;
    }
}
