using System.Collections.Generic;
using System.Threading.Tasks;
using Luppa.Data;
using System.Net.Http;
using System.Net;
using System.Xml.Linq;
using System.Linq;
using MongoDB.Driver;
using System.IO;
using System;

namespace Luppa.Crawler
{
    public class BuscapeCrawler
    {
        private MongoCollections collections;

        public BuscapeCrawler()
        {
            this.collections = new MongoCollections();
        }

        public async Task ParseLinks(List<Bidding> biddings)
        {
            foreach (var bidding in biddings)
            {
                try
                {
                    var newBidding = await ParseLink(bidding);

                    await collections
                        .Bidding
                        .ReplaceOneAsync(
                            Builders<Bidding>.Filter.Eq(t => t.Id, newBidding.Id),
                            newBidding
                        );
                }
                catch (System.Exception e)
                {
                    System.Console.WriteLine(e?.Message ?? "Error unhandled");
                }
            }    
        }

        private async Task<Bidding> ParseLink(Bidding bidding)
        {
            bidding.Score = 0;
            bidding.CrawlerPrice = 0;

            foreach (var product in bidding.Products)
            {
                product.CrawlerPrice = 0;
                product.TotalCrawlerPrice = 0;

                var buscapeLink = $"http://www.buscape.com.br/produtos?produto={product.ProductName}";
                var productBody = await HtmlGetter.GetBodyFrom(buscapeLink);

                if (string.IsNullOrEmpty(productBody) || productBody.Contains("Não era o que você procurava?"))
                    continue;

                var mostRelevantProduct = TagGetter.GetValueFromTag(
                    body: productBody,
                    tagName: "span",
                    query: ".*?class=\"bui-price__value.*?\""
                );

                if (string.IsNullOrEmpty(mostRelevantProduct))
                    continue;

                product.CrawlerUrl = buscapeLink;
                product.CrawlerPrice = double.Parse(mostRelevantProduct.Replace("R$", "").Trim());
                product.TotalCrawlerPrice = product.CrawlerPrice * product.Quantity;
                bidding.CrawlerPrice += product.TotalCrawlerPrice;

                if (product.CrawlerPrice > 0)
                    product.Score = (int)Math.Round(((product.TotalPrice - product.TotalCrawlerPrice) / product.TotalCrawlerPrice) * 100, 0);
            }

            if (bidding.CrawlerPrice > 0)
                bidding.Score = (int)Math.Round(((bidding.TotalPrice - bidding.CrawlerPrice) / bidding.CrawlerPrice) * 100, 0);
            
            return bidding;
        }

        private async Task CloseLink(CrawlerLink link)
        {
            link.IsBecParsed = true;

            await collections
                .CrowlerLink
                .ReplaceOneAsync(Builders<CrawlerLink>.Filter.Eq(t => t.Id, link.Id), link);
        }
    }
}