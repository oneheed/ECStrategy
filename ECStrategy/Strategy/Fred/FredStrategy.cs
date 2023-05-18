using ECStrategy.Models.Base;
using ECStrategy.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ECStrategy.Strategy.Fred
{
    public class FredStrategy : BaseStrategy<Request, Response>
    {
        public FredStrategy(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(nameof(FredStrategy));
        }

        public override async Task HttpRequestMessageAsync()
        {
            var test = _crawlerFieldConfig.Extra;


            _request = new Request
            {
                Sid = _crawlerFieldConfig.Extra.TryGetValue(nameof(Request.Sid), out var sid) ? sid : string.Empty,
                Hash = _crawlerFieldConfig.Extra.TryGetValue(nameof(Request.Hash), out var hash) ? hash : string.Empty,
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

                    var stream = await response.Content.ReadAsStringAsync();
                    var jsonDocument = JsonConvert.DeserializeObject<JObject>(stream);

                    //var timestamp = jsonDocument.SelectTokens("$.chart.result[0].timestamp[*]").Values<int>().ToArray();
                    var tokens = jsonDocument.SelectTokens(_crawlerFieldConfig.DataSource);
                    var values = tokens.Values().Select(token => token.ToObject<decimal?[]>());

                    //var values = jsonDocument.SelectTokens(_crawlerFieldConfig.DataSource).Values<decimal[]>().ToArray();

                    var result = values.Select((c, i) => (Date: ((long)c[0]).TimestampsToDateTime(), Value: c[1]?.ToString("0.0000")));


                    return result
                        .Where(r => r.Date >= _dateRange.StartDate && r.Date <= _dateRange.EndDate)
                        .ToDictionary(x => x.Date.ToString("yyyy-MM-dd"), x => x.Value);
                }
            }
            catch (Exception ex)
            {
                return new Dictionary<string, string>();
            }
        }
    }
}
