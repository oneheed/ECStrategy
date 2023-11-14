using ECStrategy.Models.Base;
using HtmlAgilityPack;

namespace ECStrategy.Strategy.CBC
{
    public class CBCStrategy : BaseStrategy<Request, Response>
    {
        public CBCStrategy(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(nameof(CBCStrategy));
        }
        public override async Task HttpRequestMessageAsync()
        {
            await Task.CompletedTask;
        }

        public override async Task<IDictionary<string, string>> SendRequest()
        {
            try
            {
                var pageUrl = $"{_httpClient.BaseAddress}{_crawlerFieldConfig.Route}";
                var web = new HtmlWeb();
                var doc = web.Load(pageUrl);
                var rows = doc.DocumentNode.SelectNodes("//table[@class='rwd-table']/tr");
                rows.RemoveAt(0);

                var result = new List<(DateTime Date, string Value)>();

                foreach (var row in rows)
                {
                    var data = row.SelectNodes("td/span");

                    var dateTime = DateTime.Parse(data[0].InnerHtml);
                    var rediscountRate = data[1].InnerHtml;
                    var facilityRate = data[2].InnerHtml;
                    var accommodationRate = data[3].InnerHtml;

                    dateTime = dateTime.AddDays(-dateTime.Day + 1);
                    result.Add((Date: dateTime.AddMonths(2), Value: rediscountRate));
                    result.Add((Date: dateTime.AddMonths(1), Value: rediscountRate));
                    result.Add((Date: dateTime, Value: rediscountRate));
                }

                return await Task.FromResult(result
                    .Where(r => r.Date >= _dateRange.StartDate && r.Date <= _dateRange.EndDate)
                    .ToDictionary(x => x.Date.ToString("yyyy-MM-dd"), x => x.Value));
            }
            catch (Exception ex)
            {
                return new Dictionary<string, string>();
            }
        }
    }
}
