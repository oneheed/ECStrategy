// See https://aka.ms/new-console-template for more information
using ECStrategy.Interfaces;
using ECStrategy.Models;
using ECStrategy.Strategy.CBC;
using ECStrategy.Strategy.FED;
using ECStrategy.Strategy.Fred;
using ECStrategy.Strategy.Investing;
using ECStrategy.Strategy.Macromicro;
using ECStrategy.Strategy.SalaryGrowth;
using ECStrategy.Strategy.Sbcharts;
using ECStrategy.Strategy.Yahoo;
using ECStrategy.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

var serviceCollection = new ServiceCollection();


var dateRangeConfig = configuration.GetSection("DateRange").Get<string[]>();
var startDate = DateTime.Parse(dateRangeConfig[0]);
var endDate = DateTime.Parse(dateRangeConfig[1]);

var dateRange = new DateRange
{
    StartDate = startDate,
    EndDate = endDate,
};

serviceCollection.AddScoped<IConfiguration>(_ => configuration);

Dictionary<string, Type> dict = new()
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

serviceCollection.AddScoped<YahooStrategy>();

foreach (var item in dict)
{
    serviceCollection.AddScoped(item.Value);
}

Configure(serviceCollection);

static void Configure(IServiceCollection services)
{
    var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
    var urls = configuration.GetSection("Crawler:URLs").Get<IDictionary<string, string>>();

    foreach (var url in urls)
    {
        services.AddHttpClient(CommandUtility.GetStrategyName(url.Key), c =>
        {
            c.BaseAddress = new Uri(url.Value);
        });
    }
}

var fields = configuration.GetSection("Crawler:Fields").Get<Dictionary<string, CrawlerFieldConfig>>();


var csvResult = new Dictionary<(DateTime Start, DateTime End), JObject>();

for (var i = dateRange.EndDate; i >= dateRange.StartDate;)
{
    csvResult.Add((i.AddMonths(-1), i), new JObject());

    i = i.AddMonths(-1);
}

foreach (var field in fields)
{

    if (dict.TryGetValue(CommandUtility.GetStrategyName(field.Value.Strategy), out var type))
    {
        var strategy = (IStrategy)serviceCollection.BuildServiceProvider().GetService(type);
        await strategy.Init(field.Key, dateRange, field.Value);
        await strategy.HttpRequestMessageAsync();

        var result = await strategy.SendRequest();

        foreach (var item in result)
        {
            var data = csvResult.FirstOrDefault(c => DateTime.Parse(item.Key) >= c.Key.Start && DateTime.Parse(item.Key) < c.Key.End);

            data.Value[field.Key] = item.Value;
        }

        Console.WriteLine(field.Key);
        Console.WriteLine(JsonConvert.SerializeObject(result));
    }
}

Console.WriteLine("=================================================");
Console.WriteLine(JsonConvert.SerializeObject(csvResult));
Console.ReadLine();