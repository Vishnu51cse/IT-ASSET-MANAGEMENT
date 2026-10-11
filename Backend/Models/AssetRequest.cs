using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class AssetRequest
    {
        [Key]
        public int RequestId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public Employee? Employee { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public AssetCategory? Category { get; set; }

        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;

        public DateTime RequestedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Completed

        [MaxLength(500)]
        public string AdminComment { get; set; } = string.Empty;
    }
}
