using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string EmployeeCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Department { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Designation { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Active"; // Active, Deactivated

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public User? User { get; set; }
        public ICollection<AssetAssignment> Assignments { get; set; } = new List<AssetAssignment>();
        public ICollection<AssetRequest> AssetRequests { get; set; } = new List<AssetRequest>();
    }
}
