using ECStrategy.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json.Linq;

namespace ECStrategy.Utilities
{
    public static class GoogleSheetUtility
    {
        private static readonly string[] scopes = new string[] { SheetsService.Scope.Spreadsheets };
        private static readonly string applicationName = "Update Google Sheet Data with Google Sheets API v4";
        private static readonly string spreadsheetId = "13mKmX8_rKaEbWCxfRAcCQohhbS3ytSnRLLYAFN_6Nww";
        private static readonly string sheetName = "工作表1";

        private static GoogleCredential credential;
        private static SheetsService service;

        static GoogleSheetUtility()
        {
            using (var stream = new FileStream("Extension/GoogleCredential.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(scopes);
            }

            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName,
            });
        }

        public static void SetTitle(Dictionary<string, CrawlerFieldConfig> crawlerFields)
        {
            var first = GetColumnName(2);
            var end = GetColumnName(2 + crawlerFields.Count);

            var range = $"{sheetName}!{first}1:{end}1";
            var valueRage = new ValueRange();

            var objectList = crawlerFields.OrderBy(x => x.Value.Order).Select(x => (object)x.Value.Name).ToList();
            valueRage.Values = new List<IList<object>> { objectList };

            var updateRequest = service.Spreadsheets.Values.Update(valueRage, spreadsheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

            _ = updateRequest.Execute();
        }

        public static void SetDate(DateRange dateRange)
        {
            var count = GetMonthsBetween(dateRange.StartDate, dateRange.EndDate);
            var objectList = Enumerable.Range(0, count).Select(i => new List<object> { (object)dateRange.EndDate.AddMonths(-i).ToString("yyyy-MM") }).ToList();

            SetDate("A", objectList);
        }

        public static void SetData(List<JObject> pairs, Dictionary<string, CrawlerFieldConfig> crawlerFields)
        {
            var rows = new List<IList<object>>();

            foreach (var item in pairs)
            {
                var cols = new List<object>();
                foreach (var field in crawlerFields.OrderBy(x => x.Value.Order))
                {
                    var text = item.TryGetValue(field.Key, out var token) ? token.ToString() : string.Empty;

                    cols.Add(text);
                }

                rows.Add(cols);
            }

            var end = GetColumnName(1 + crawlerFields.Count);

            var range = $"{sheetName}!B2:{end}{2 + rows.Count}";

            var valueRage = new ValueRange();
            valueRage.Values = rows;

            var updateRequest = service.Spreadsheets.Values.Update(valueRage, spreadsheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

            _ = updateRequest.Execute();
        }

        public static void SetDate(string columnName, List<List<object>> objectList)
        {
            var range = $"{sheetName}!{columnName}2:{columnName}{2 + objectList.Count}";
            var valueRage = new ValueRange();

            valueRage.Values = new List<IList<object>>(objectList);

            var updateRequest = service.Spreadsheets.Values.Update(valueRage, spreadsheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

            _ = updateRequest.Execute();
        }

        public static int GetMonthsBetween(DateTime from, DateTime to)
        {
            if (from >= to) return GetMonthsBetween(to, from);

            var monthDiff = Math.Abs((to.Year * 12 + (to.Month - 1)) - (from.Year * 12 + (from.Month - 1)));

            if (from.AddMonths(monthDiff) > to || to.Day < from.Day)
            {
                return monthDiff - 1;
            }
            else
            {
                return monthDiff;
            }
        }


        public static string GetColumnName(int columnNumber)
        {
            string columnName = "";

            while (columnNumber > 0)
            {
                int remainder = (columnNumber - 1) % 26;
                columnName = (char)('A' + remainder) + columnName;
                columnNumber = (columnNumber - 1) / 26;
            }

            return columnName;
        }

        public static void Test()
        {
            var range = $"{sheetName}!A:F";
            var valueRage = new ValueRange();

            var objectList = new List<object>() { "Hello!", "This", "was", "inserted", "test", "C#" };
            valueRage.Values = new List<IList<object>> { objectList };

            var appendRequest = service.Spreadsheets.Values.Append(valueRage, spreadsheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

            var appendResponse = appendRequest.Execute();
        }
    }
}
