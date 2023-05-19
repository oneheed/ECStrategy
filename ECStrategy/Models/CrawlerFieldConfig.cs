namespace ECStrategy.Models
{
    public class CrawlerFieldConfig
    {
        public int Order { get; set; } = 0;

        public string Name { get; set; } = string.Empty;

        public string Strategy { get; set; } = string.Empty;

        public string Route { get; set; } = string.Empty;

        public string DataSource { get; set; } = string.Empty;

        public string TimeSource { get; set; } = string.Empty;

        public Dictionary<string, string> Extra { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
}
