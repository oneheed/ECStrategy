using ECStrategy.Models.Base;

namespace ECStrategy.Strategy.Macromicro
{
    public class Request : BaseRequest
    {
        public override string StartDate { get; set; }

        public override string EndDate { get; set; }
    }
}
