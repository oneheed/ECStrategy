using ECStrategy.Models.Base;
using Newtonsoft.Json;

namespace ECStrategy.Strategy.Yahoo
{
    public class Request : BaseRequest
    {
        public string Formatted { get; set; } = "true";

        public string Crumb { get; set; } = "8f93uFQu21q";

        public string Lang { get; set; } = "en-US";

        public string Region { get; set; } = "US";

        public string IncludeAdjustedClose { get; set; } = "true";

        public string Interval { get; set; } = "1mo";

        [JsonProperty("Period1")]
        public override string StartDate { get; set; }

        [JsonProperty("Period2")]
        public override string EndDate { get; set; }

        public string Events { get; set; } = "capitalGain|div|split";

        public string UseYfid { get; set; } = "true";

        public string CorsDomain { get; set; } = "finance.yahoo.com";
    }
}
