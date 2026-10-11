using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class AssetAssignment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AssetId { get; set; }

        [ForeignKey(nameof(AssetId))]
        public Asset? Asset { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public Employee? Employee { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ExpectedReturnDate { get; set; }

        public DateTime? ActualReturnDate { get; set; }

        [MaxLength(100)]
        public string AssignedBy { get; set; } = "Admin";

        [MaxLength(500)]
        public string Notes { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "Active"; // Active, Returned
    }
}
