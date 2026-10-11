namespace Backend.DTOs
{
    public class AssetHistoryDto
    {
        public int HistoryId { get; set; }
        public int AssetId { get; set; }
        public string AssetTag { get; set; } = string.Empty;
        public string AssetName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string PerformedBy { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
