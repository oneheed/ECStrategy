using ECStrategy.Models.Base;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ECStrategy.Strategy.FED
{
    public class FEDStrategy : BaseStrategy<Request, Response>
    {
        public FEDStrategy(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(nameof(FEDStrategy));
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
                var rows = doc.DocumentNode.SelectSingleNode("//script[contains(., \"timeLines\")]/text()");

                var json = ParseVarParams(rows.InnerHtml);

                var jsonObject = JsonConvert.DeserializeObject<JObject>(json);
                var tokens = jsonObject.SelectTokens("$.11.settings_json.label");
                var values = tokens.Values().Select(token => token.ToObject<Response>());

                var groups = values.GroupBy(v => v.Date.Substring(0, 7));

                var result = groups.Select(g =>
                {
                    return (Date: DateTime.Parse(g.Key), Value: string.Join("\r\n", g.Select(i => $"{i.Data.Title}:{i.Data.Desc}")));
                }).ToList();

                return await Task.FromResult(result
                    .Where(r => r.Date >= _dateRange.StartDate && r.Date <= _dateRange.EndDate)
                    .ToDictionary(x => x.Date.ToString("yyyy-MM-dd"), x => x.Value));
            }
            catch (Exception ex)
            {
                return new Dictionary<string, string>();
            }
        }

        string ParseVarParams(string scriptContent)
        {
            // 移除空白字元和換行符號
            scriptContent = scriptContent.Replace(" ", "").Replace("\n", "");
            var fistIndex = scriptContent.IndexOf("vartimeLines=");
            scriptContent = scriptContent.Remove(0, fistIndex + "vartimeLines=".Length);
            var lastIndex = scriptContent.IndexOf(";");
            scriptContent = scriptContent.Remove(lastIndex, scriptContent.Length - lastIndex);


            return scriptContent;
        }
    }
}
