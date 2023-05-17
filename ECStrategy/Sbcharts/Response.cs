using Newtonsoft.Json;

namespace ECStrategy.Sbcharts
{
    public class Response
    {
        public long Timestamp { get; set; }

        [JsonProperty("actual_state")]
        public string ActualState { get; set; } = string.Empty;

        public decimal? Actual { get; set; }

        [JsonProperty("actual_formatted")]
        public string ActualFormatted { get; set; } = string.Empty;

        public decimal? Forecast { get; set; }

        [JsonProperty("forecast_formatted")]
        public string ForecastFormatted { get; set; } = string.Empty;

        public decimal? Revised { get; set; }

        [JsonProperty("Revised_formatted")]
        public string RevisedFormatted { get; set; } = string.Empty;
    }
}
