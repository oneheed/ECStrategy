using ECStrategy.Models.Base;
using ECStrategy.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ECStrategy.Strategy.Yahoo
{
    public class YahooStrategy : BaseStrategy<Request, Response>
    {
        public YahooStrategy(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(nameof(YahooStrategy));
        }

        public override async Task HttpRequestMessageAsync()
        {
            var startTimestamps = (long)_dateRange.StartDate.Subtract(DateTime.UnixEpoch).TotalSeconds;
            var endTimestamps = (long)_dateRange.EndDate.Subtract(DateTime.UnixEpoch).TotalSeconds;

            _request = new Request
            {
                StartDate = startTimestamps.ToString(),
                EndDate = endTimestamps.ToString(),
            };

            _httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_crawlerFieldConfig.Route}?{_request.RequestToQueryString()}");

            await Task.CompletedTask;
        }

        public override async Task<IDictionary<string, string>> SendRequest()
        {
            try
            {
                using (var response = await _httpClient.SendAsync(_httpRequestMessage))
                {
                    response.EnsureSuccessStatusCode();

                    var startDate = _dateRange.StartDate;
                    var stream = await response.Content.ReadAsStringAsync();
                    var jsonDocument = JsonConvert.DeserializeObject<JObject>(stream);

                    //var timestamp = jsonDocument.SelectTokens("$.chart.result[0].timestamp[*]").Values<int>().ToArray();
                    var close = jsonDocument.SelectTokens(_crawlerFieldConfig.DataSource).Values<decimal>().ToArray();

                    var result = close.Select((c, i) => (Date: startDate.AddMonths(i), Value: c.ToString("0.0000")));

                    return result.ToDictionary(x => x.Date.ToString("yyyy-MM-dd"), x => x.Value);
                }
            }
            catch (Exception ex)
            {
                return new Dictionary<string, string>();
            }
        }
    }
}
