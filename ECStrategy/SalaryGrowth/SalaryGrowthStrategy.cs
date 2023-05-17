using HtmlAgilityPack;

namespace ECStrategy.SalaryGrowth
{
    public class SalaryGrowthStrategy : BaseStrategy<Request, Response>
    {
        public SalaryGrowthStrategy(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(nameof(SalaryGrowthStrategy));
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

                    var doc = new HtmlDocument();
                    doc.LoadHtml(stream);

                    var values = doc.DocumentNode.SelectNodes("//*[@data-chartid='143451']/div[@class='figBorder']/div[@class='figInner']/div[contains(@class, 'data-table-wrapper')]/table/tbody/tr");

                    var result = new List<(DateTime Date, string Value)>();

                    foreach (var value in values)
                    {
                        var data = value.InnerText.Split("\n").Where(r => !string.IsNullOrEmpty(r)).ToList();
                        var dateTime = DateTime.Parse(data[0]);
                        var nonfarmEmployees = data[1];
                        var workers = data[2];

                        result.Add((Date: dateTime, Value: workers));
                    }

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
