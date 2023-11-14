// See https://aka.ms/new-console-template for more information
using ECStrategy.Interfaces;
using ECStrategy.Models;
using ECStrategy.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

var serviceCollection = new ServiceCollection();
serviceCollection.AddScoped<IConfiguration>(_ => configuration);

// DateRange
var dateRangeConfig = configuration.GetSection("DateRange").Get<string[]>();
var startDate = DateTime.Parse(dateRangeConfig[0]);
var endDate = DateTime.Parse(dateRangeConfig[1]);

var dateRange = new DateRange
{
    StartDate = startDate,
    EndDate = endDate,
};

serviceCollection.StrategyConfigure();
serviceCollection.HttpClientConfigure();


var crawlerFields = configuration.GetSection("Crawler:Fields").Get<Dictionary<string, CrawlerFieldConfig>>();


var csvResult = new Dictionary<(DateTime Start, DateTime End), JObject>();

for (var i = dateRange.EndDate.AddMonths(1); i >= dateRange.StartDate;)
{
    csvResult.Add((i.AddMonths(-1), i), new JObject());

    i = i.AddMonths(-1);
}

GoogleSheetUtility.SetTitle(crawlerFields);
GoogleSheetUtility.SetDate(dateRange);

var serviceProvider = serviceCollection.BuildServiceProvider();

foreach (var field in crawlerFields)
{
    var key = CommandUtility.GetStrategyName(field.Value.Strategy);
    if (ServiceCollectionUtility.Strategies.TryGetValue(key, out var type))
    {
        var strategy = (IStrategy)serviceProvider.GetService(type);
        strategy.Init(field.Key, dateRange, field.Value);
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

GoogleSheetUtility.SetData(csvResult.Values.ToList(), crawlerFields);

Console.WriteLine("=================================================");
Console.WriteLine(JsonConvert.SerializeObject(csvResult));
Console.ReadLine();