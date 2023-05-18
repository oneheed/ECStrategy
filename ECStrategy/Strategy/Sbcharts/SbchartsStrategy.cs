// Ignore Spelling: Sbcharts

using ECStrategy.Models.Base;
using ECStrategy.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ECStrategy.Strategy.Sbcharts
{
    public class SbchartsStrategy : BaseStrategy<Request, Response>
    {
        public SbchartsStrategy(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(nameof(SbchartsStrategy));
        }

        public override async Task HttpRequestMessageAsync()
        {
            _httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_crawlerFieldConfig.Route}");

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

                    var tokens = jsonDocument.SelectTokens(_crawlerFieldConfig.DataSource);
                    var values = tokens.Values().Select(token => token.ToObject<Response>());

                    var result = default(IDictionary<DateTime, string>);

                    switch (_fieldName)
                    {
                        case "UnemploymentBenefits":
                            var group = values
                               .Where(v => v.Timestamp.TimestampsToDateTime() >= _dateRange.StartDate &&
                                   v.Timestamp.TimestampsToDateTime() <= _dateRange.EndDate)
                               .GroupBy(v => v.Timestamp.TimestampsToDateTime().ToString("yyyy-MM")).ToList();

                            result = group.ToDictionary(g => g.First().Timestamp.TimestampsToDateTime(), v => v.First().Actual?.ToString("0.0000"));

                            break;

                        default:
                            result = values
                               .Where(v => v.Timestamp.TimestampsToDateTime().AddMonths(-1) >= _dateRange.StartDate &&
                                   v.Timestamp.TimestampsToDateTime().AddMonths(-1) <= _dateRange.EndDate)
                               .ToDictionary(v => v.Timestamp.TimestampsToDateTime().AddMonths(-1), v => v.Actual?.ToString("0.0000"));

                            break;
                    }


                    return result.ToDictionary(x => x.Key.ToString("yyyy-MM-dd"), x => x.Value);
                }
            }
            catch (Exception ex)
            {
                return new Dictionary<string, string>();
            }
        }
    }
}
