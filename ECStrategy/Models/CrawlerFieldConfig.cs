namespace ECStrategy.Models
{
    public class CrawlerFieldConfig
    {
        public string Strategy { get; set; } = string.Empty;

        public string Route { get; set; } = string.Empty;

        public string DataSource { get; set; } = string.Empty;

        public string TimeSource { get; set; } = string.Empty;

        public Dictionary<string, string> Extra { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
}
