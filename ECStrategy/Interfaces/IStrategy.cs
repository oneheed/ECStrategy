namespace ECStrategy.Interfaces
{
    internal interface IStrategy
    {
        Task Init(string fieldName, DateRange dateRange, CrawlerFieldConfig crawlerFieldConfig);

        Task HttpRequestMessageAsync();

        Task<IDictionary<string, string>> SendRequest();
    }
}
