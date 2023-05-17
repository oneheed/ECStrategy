using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace ECStrategy.Investing
{
    public class InvestingStrategy : BaseStrategy<Request, Response>
    {
        public InvestingStrategy(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(nameof(InvestingStrategy));
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
                var options = new ChromeOptions();
                //options.AddArgument("--ignore-certificate-errors");
                //options.AddArgument("--ignore-ssl-errors");
                options.BinaryLocation = "C:\\Program Files\\BraveSoftware\\Brave-Browser\\Application\\brave.exe";

                var driver = new ChromeDriver(options);
                var js = (IJavaScriptExecutor)driver;
                driver.Navigate().GoToUrl("https://www.investing.com/rates-bonds/u.s.-10-year-bond-yield-historical-data");
                driver.Manage().Window.Maximize();
                js.ExecuteScript("window.scrollTo(0,800)");
                driver.FindElement(By.XPath("//div[@id='history-timeframe-selector']/div/div")).Click();
                driver.FindElement(By.XPath("//div[@id=\'history-timeframe-selector\']/div[2]/div/div[3]")).Click();
                driver.FindElement(By.XPath("//div[2]/div[2]/div[2]/div/div")).Click();
                driver.FindElement(By.CssSelector(".NativeDateInput_root__wbgyP:nth-child(1) > input")).Click();
                driver.FindElement(By.CssSelector(".NativeDateInput_root__wbgyP:nth-child(1) > input")).SendKeys("2021-05-01");
                driver.FindElement(By.CssSelector(".NativeDateInput_root__wbgyP:nth-child(2) > input")).Click();
                driver.FindElement(By.CssSelector(".NativeDateInput_root__wbgyP:nth-child(2) > input")).SendKeys("2023-05-03");
                driver.FindElement(By.CssSelector(".HistoryDatePicker_apply-button__fPr_G")).Click();

                var stream = driver.PageSource;

                driver.Quit();
                driver.Dispose();

                var doc = new HtmlDocument();
                doc.LoadHtml(stream);

                var values = doc.DocumentNode.SelectNodes("//tr[@data-test='historical-data-table-row']");

                var result = new List<(DateTime Date, string Value)>();
                values.Remove(values.Last());

                foreach (var value in values)
                {
                    var data = value.SelectNodes("td");
                    var dateTime = DateTime.Parse(data[0].SelectSingleNode("time").InnerHtml);
                    var price = data[1].InnerHtml;
                    var open = data[2].InnerHtml;
                    var high = data[3].InnerHtml;
                    var low = data[4].InnerHtml;
                    var change = data[5].InnerHtml;

                    result.Add((Date: dateTime, Value: price));
                }

                return result
                        .Where(r => r.Date >= this._dateRange.StartDate && r.Date <= this._dateRange.EndDate)
                        .ToDictionary(x => x.Date.ToString("yyyy-MM-dd"), x => x.Value);
            }
            catch (Exception ex)
            {
                return new Dictionary<string, string>();
            }
        }
    }
}
