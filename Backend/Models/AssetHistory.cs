using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class AssetHistory
    {
        [Key]
        public int HistoryId { get; set; }

        [Required]
        public int AssetId { get; set; }

        [ForeignKey(nameof(AssetId))]
        public Asset? Asset { get; set; }

        [Required]
        [MaxLength(100)]
        public string Action { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string PerformedBy { get; set; } = string.Empty;

        public DateTime Date { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}
