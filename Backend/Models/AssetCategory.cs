using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class AssetCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(250)]
        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<Asset> Assets { get; set; } = new List<Asset>();
    }
}
