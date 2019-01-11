namespace TcgPriceCrawler
{
    class CardPrice
    {
        public string SetName { get; set; }
        public string CardName { get; set; }
        public double MarketPrice { get; set; }
        public double BuylistMarketPrice { get; set; }
        public double MedianPrice { get; set; }
    }
}
