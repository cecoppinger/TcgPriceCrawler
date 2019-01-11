using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace TcgPriceCrawler
{
    class TcgPriceCrawler
    {
        private HtmlWeb _webLoader;
        private readonly string _baseUrl = @"https://shop.tcgplayer.com/price-guide/magic";
        private Dictionary<string, string> _selectionStrings = new Dictionary<string, string>();
        private List<string> _setNames = new List<string>();

        public TcgPriceCrawler()
        {
            _webLoader = new HtmlWeb();
            LoadSetNames(_webLoader);
            _selectionStrings.Add("marketPrices", "//td[@class='marketPrice']/div");
            _selectionStrings.Add("buylistMarketPrices", "//td[@class='buylistMarketPrice']/div");
            _selectionStrings.Add("medianPrices", "//td[@class='medianPrice']/div");
            _selectionStrings.Add("cardNames", "//div[@class='productDetail']/a");
            _selectionStrings.Add("setNames", "//select[@id='set']/option");
        }

        public List<CardPrice> DoIt()
        {
            List<CardPrice> cardPrices = new List<CardPrice>();
            string currentEndpoint;

            foreach(string set in _setNames)
            {
                currentEndpoint = _baseUrl + "/" + Format(set);
                HtmlDocument doc = _webLoader.Load(currentEndpoint);

                var cardNameNodes = doc.DocumentNode.SelectNodes(_selectionStrings["cardNames"]);
                var marketPriceNodes = doc.DocumentNode.SelectNodes(_selectionStrings["marketPrices"]);
                var buylistMarketPriceNodes = doc.DocumentNode.SelectNodes(_selectionStrings["buylistMarketPrices"]);
                var medianPriceNodes = doc.DocumentNode.SelectNodes(_selectionStrings["medianPrices"]);

                if (cardNameNodes == null)
                    continue;
                else
                {
                    for (int i = 0; i < cardNameNodes.Count; i++)
                    {
                        CardPrice newCard = new CardPrice()
                        {
                            SetName = set,
                            CardName = ReplaceEncodedText(cardNameNodes[i].InnerHtml.Trim()),
                            MarketPrice = Validate(marketPriceNodes[i]),
                            MedianPrice = Validate(medianPriceNodes[i]),
                            BuylistMarketPrice = Validate(buylistMarketPriceNodes[i])
                        };

                        cardPrices.Add(newCard);
                    }
                }
            }

            return cardPrices;
        }

        private void LoadSetNames(HtmlWeb webLoader)
        {
            HtmlDocument doc = webLoader.Load(_baseUrl);
            var nodes = doc.DocumentNode.SelectNodes(_selectionStrings["setNames"]);
            string setName;
            foreach (var node in nodes)
            {
                setName = node.InnerHtml;
                _setNames.Add(ReplaceEncodedText(setName));
            }
        }

        //encoded text was throwing off my formatting
        private string ReplaceEncodedText(string str)
        {
            Regex rgx = new Regex(@"(&#39;)");
            string newString = rgx.Replace(str, "'");
            return newString;
        }

        private string Format(string myString)
        {
            StringBuilder sb = new StringBuilder();
            foreach(char c in myString.ToLower())
            {
                if (c == ' ')
                    sb.Append("-");
                else if (char.IsLetterOrDigit(c))
                    sb.Append(c);
                else
                    continue;
            }
            return sb.ToString();
        }

        private double Validate(HtmlNode node)
        {
            if (double.TryParse(node.InnerHtml.Trim().Remove(0, 1), out double validated))
                return validated;
            else
                return 0;
        }
    }
}
