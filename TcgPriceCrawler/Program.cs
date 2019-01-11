using System;
using System.Collections.Generic;
using System.IO;

namespace TcgPriceCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            List<CardPrice> prices;
            TcgPriceCrawler crawler = new TcgPriceCrawler();
            prices = crawler.DoIt();
            Console.WriteLine("prices fetched");
            Console.ReadKey();
            using (StreamWriter file = new StreamWriter(@"D:\Documents\lc101\random-things\TcgPriceCrawler\TcgPriceCrawler\TextFile1.txt"))
            {
                foreach (CardPrice card in prices)
                {
                    Console.WriteLine("creating new entry...");
                    file.WriteLine("{0}, {1}, {2}, {3}",
                    card.CardName,
                    card.MarketPrice,
                    card.MedianPrice,
                    card.BuylistMarketPrice);
                }
            }
        }
    }
}
