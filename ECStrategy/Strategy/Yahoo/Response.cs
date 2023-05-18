namespace ECStrategy.Strategy.Yahoo
{
    public class Response
    {
        public Chart Chart { get; set; }
    }

    public class Chart
    {
        public IEnumerable<Result> Result { get; set; }


        public string Error { get; set; }
    }

    public class Result
    {
        public Meta Meta { get; set; }

        public IEnumerable<int> Timestamp { get; set; }

        public Events Events { get; set; }

        public Indicators Indicators { get; set; }
    }

    public class Meta
    {
        public string Currency { get; set; }

        public string Symbol { get; set; }

        public string ExchangeName { get; set; }

        public string InstrumentType { get; set; }

        public int FirstTradeDate { get; set; }

        public int RegularMarketTime { get; set; }

        public int Gmtoffset { get; set; }

        public string Timezone { get; set; }

        public string ExchangeTimezoneName { get; set; }

        public decimal RegularMarketPrice { get; set; }

        public decimal RhartPreviousClose { get; set; }

        public int PriceHint { get; set; }

        public CurrentTradingPeriod CurrentTradingPeriod { get; set; }

        public string DataGranularity { get; set; }

        public string Range { get; set; }

        public IEnumerable<string> ValidRanges { get; set; }
    }

    public class CurrentTradingPeriod
    {
        public TradingPeriod Pre { get; set; }

        public TradingPeriod Regular { get; set; }

        public TradingPeriod Post { get; set; }
    }

    public class TradingPeriod
    {
        public string TimeZone { get; set; }

        public int Start { get; set; }

        public int End { get; set; }

        public int GmtOffset { get; set; }
    }

    public class Events
    {
        public IDictionary<string, Dividends> Dividends { get; set; }
    }


    public class Dividends
    {
        public decimal Amount { get; set; }

        public TimeSpan Date { get; set; }
    }

    public class Indicators
    {
        public IEnumerable<QuoteData> Quote { get; set; }

        public IEnumerable<AdjCloseData> AdjClose { get; set; }
    }

    public class QuoteData
    {
        public IEnumerable<decimal?> Open { get; set; }
        public IEnumerable<decimal?> Low { get; set; }
        public IEnumerable<decimal?> Close { get; set; }
        public IEnumerable<int?> Volume { get; set; }
        public IEnumerable<decimal?> High { get; set; }
    }

    public class AdjCloseData
    {
        public IEnumerable<decimal?> AdjClose { get; set; }
    }

}
