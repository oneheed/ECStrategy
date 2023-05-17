using ECStrategy.Interfaces;

namespace ECStrategy
{
    public abstract class BaseStrategy<TRequest, TResponse> : IStrategy
    {
        protected HttpClient _httpClient;

        protected TRequest _request;

        protected HttpRequestMessage _httpRequestMessage;

        protected DateRange _dateRange;

        protected string _fieldName;

        protected CrawlerFieldConfig _crawlerFieldConfig;

        public async Task Init(string fieldName, DateRange dateRange, CrawlerFieldConfig crawlerFieldConfig)
        {
            _fieldName = fieldName;
            _dateRange = dateRange;
            _crawlerFieldConfig = crawlerFieldConfig;
        }

        public abstract Task HttpRequestMessageAsync();

        public abstract Task<IDictionary<string, string>> SendRequest();
    }
}
