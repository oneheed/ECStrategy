using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

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
