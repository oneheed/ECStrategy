using ECStrategy.Models.Base;

namespace ECStrategy.Strategy.Fred
{
    public class Request : BaseRequest
    {
        public string Obs { get; set; } = "true";

        public string Sid { get; set; }

        public string Hash { get; set; }
    }
}
