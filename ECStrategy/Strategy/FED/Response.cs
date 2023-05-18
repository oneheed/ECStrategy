namespace ECStrategy.Strategy.FED
{
    public class Response
    {
        public string Date { get; set; }

        public Data Data { get; set; }

        public string Style { get; set; }

        public string Position { get; set; }
    }

    public class Data
    {
        public string Title { get; set; }

        public string Desc { get; set; }
    }
}
