// Ignore Spelling: Macromicro

using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ECStrategy.Macromicro
{
    public class MacromicroStrategy : BaseStrategy<Request, Response>
    {
        private IHttpClientFactory _httpClientFactory;

        public MacromicroStrategy(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient(nameof(MacromicroStrategy));
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
                var cookie = string.Empty;
                var authorization = string.Empty;

                if (_crawlerFieldConfig.Extra.TryGetValue("pageUrl", out var pageUrl))
                {
                    using (var pageResponse = await _httpClientFactory.CreateClient().GetAsync(pageUrl))
                    {
                        var cookies = pageResponse.Headers.SingleOrDefault(header => header.Key == "Set-Cookie").Value;
                        cookie = cookies.FirstOrDefault();

                        using (var content = pageResponse.Content)
                        {
                            var result = content.ReadAsStringAsync().Result;
                            var doc = new HtmlDocument();
                            doc.LoadHtml(result);

                            var value = doc.DocumentNode.SelectSingleNode("//*[@id=\"panel\"]/footer/*[@class=\"container\"]/*[@class=\"sosume\"]/p[1]").GetDataAttribute("stk");
                            authorization = value.Value;
                        }
                    }
                }
                else
                {
                    // TODO: Exception
                }

                _httpRequestMessage.Headers.Add("authorization", $"Bearer {authorization}");
                _httpRequestMessage.Headers.Add("cookie", cookie);

                using (var response = await _httpClient.SendAsync(_httpRequestMessage))
                {
                    response.EnsureSuccessStatusCode();

                    var stream = await response.Content.ReadAsStringAsync();
                    var jsonDocument = JsonConvert.DeserializeObject<JObject>(stream);

                    var tokens = jsonDocument.SelectTokens(_crawlerFieldConfig.DataSource);
                    var values = tokens.Values().Select(token => token.ToObject<string[]>());

                    var result = values.Select((c, i) => (Date: DateTime.Parse(c[0]), Value: c[1]));


                    return result
                        .Where(r => r.Date >= this._dateRange.StartDate && r.Date <= this._dateRange.EndDate)
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
