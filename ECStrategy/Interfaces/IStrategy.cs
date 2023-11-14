using ECStrategy.Models;

namespace ECStrategy.Interfaces
{
    internal interface IStrategy
    {
        void Init(string fieldName, DateRange dateRange, CrawlerFieldConfig crawlerFieldConfig);

        Task HttpRequestMessageAsync();

        Task<IDictionary<string, string>> SendRequest();
    }
}
