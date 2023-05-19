using ECStrategy.Strategy.CBC;
using ECStrategy.Strategy.FED;
using ECStrategy.Strategy.Fred;
using ECStrategy.Strategy.Investing;
using ECStrategy.Strategy.Macromicro;
using ECStrategy.Strategy.SalaryGrowth;
using ECStrategy.Strategy.Sbcharts;
using ECStrategy.Strategy.Yahoo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECStrategy.Utilities
{
    public static class ServiceCollectionUtility
    {
        private static Dictionary<string, Type> _strategies = new()
        {
            { nameof(FredStrategy), typeof(FredStrategy) },
            { nameof(YahooStrategy), typeof(YahooStrategy) },
            { nameof(MacromicroStrategy), typeof(MacromicroStrategy) },
            { nameof(SbchartsStrategy), typeof(SbchartsStrategy) },
            { nameof(SalaryGrowthStrategy), typeof(SalaryGrowthStrategy) },
            { nameof(InvestingStrategy), typeof(InvestingStrategy) },
            { nameof(CBCStrategy), typeof(CBCStrategy) },
            { nameof(FEDStrategy), typeof(FEDStrategy) },
        };

        public static void HttpClientConfigure(IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider().GetService<IConfiguration>()!;
            var urls = configuration.GetSection("Crawler:URLs").Get<IDictionary<string, string>>();

            foreach (var url in urls)
            {
                services.AddHttpClient(CommandUtility.GetStrategyName(url.Key), c =>
                {
                    c.BaseAddress = new Uri(url.Value);
                });
            }
        }

        public static Dictionary<string, Type> Strategies
            => _strategies;

        public static void StrategyConfigure(IServiceCollection services)
        {
            foreach (var strategy in _strategies)
            {
                services.AddScoped(strategy.Value);
            }
        }
    }
}
