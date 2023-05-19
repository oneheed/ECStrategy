using System.Web;
using ECStrategy.Models.Base;
using Newtonsoft.Json;

namespace ECStrategy.Utilities
{
    public static class CommandUtility
    {
        public static string GetStrategyName(string key)
            => $"{key}Strategy";

        public static string RequestToQueryString<T>(this T request) where T : BaseRequest
        {
            var step1 = JsonConvert.SerializeObject(request);

            var step2 = JsonConvert.DeserializeObject<IDictionary<string, string>>(step1);

            var step3 = step2?.Select(x => HttpUtility.UrlEncode(x.Key.ToLower()) + "=" + HttpUtility.UrlEncode(x.Value)) ?? new List<string>();

            return string.Join("&", step3);
        }

        public static long DateTimeToTimestamps(this DateTime dateTime)
            => (long)dateTime.Subtract(DateTime.UnixEpoch).TotalSeconds;

        public static DateTime TimestampsToDateTime(this long timestamps)
            => DateTime.UnixEpoch.AddMilliseconds(timestamps);
    }
}
