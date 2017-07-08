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
using Luppa.Helpers;

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
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{biddings.Count} items to parse.");

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

                System.Console.WriteLine($"Parsing {product.ProductName}...");

                var buscapeLink = $"http://www.buscape.com.br/produtos?produto={product.ProductName}";
                var productBody = string.Empty;

                product.CrawlerUrl = buscapeLink;

                try
                {
                    productBody = await HtmlGetter.GetBodyFrom(product.CrawlerUrl);

                    if (string.IsNullOrEmpty(productBody) || productBody.Contains("Não era o que você procurava?"))
                    {
                        // If "ref" exists, search by "ref"
                        if (!product.ProductName.Contains("ref.") && !product.ProductName.Contains("ref:"))
                            continue;

                        var refSplit = product
                            .ProductName
                            .Substring(product
                                .ProductName
                                .IndexOf("ref")
                            )
                            .Split(new char[]{ ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        
                        if (refSplit.Count() < 2)
                            continue;
                        
                        var refNumber = refSplit[1];

                        product.CrawlerUrl = $"http://www.buscape.com.br/produtos?produto={refNumber}";
                        productBody = await HtmlGetter.GetBodyFrom(product.CrawlerUrl);

                        if (string.IsNullOrEmpty(productBody))
                            continue;
                    }
                }
                catch (System.Exception e)
                {
                    System.Console.WriteLine(e?.Message);
                    continue;
                }

                var mostRelevantProduct = TagGetter.GetValueFromTag(
                    body: productBody,
                    tagName: "span",
                    query: ".*?class=\"bui-price__value.*?\""
                );

                if (string.IsNullOrEmpty(mostRelevantProduct))
                {
                    mostRelevantProduct = TagGetter.GetValueFromTag(
                        body: productBody,
                        tagName: "span",
                        query: ".*?data-reactid=\"80\""
                    );
                }

                if (string.IsNullOrEmpty(mostRelevantProduct))
                {
                    mostRelevantProduct = TagGetter.GetValueFromTag(
                        body: productBody,
                        tagName: "span",
                        query: ".*?class=\"featured-price__highlight\""
                    );
                }

                if (string.IsNullOrEmpty(mostRelevantProduct))
                    continue;
                
                var crawlerPrice = 0d;
                if (!double.TryParse(mostRelevantProduct.Replace("R$", "").Trim(), out crawlerPrice))
                    continue;

                product.CrawlerPrice = crawlerPrice;
                product.TotalCrawlerPrice = product.CrawlerPrice * product.Quantity;
                bidding.CrawlerPrice += product.TotalCrawlerPrice;

                if (product.CrawlerPrice > 0)
                    product.Score = (int)Math.Round(((product.TotalPrice - product.TotalCrawlerPrice) / product.TotalCrawlerPrice) * 100, 0);
            }

            // Total only when CrawlerPrice > 0
            bidding.TotalPrice = bidding
                .Products
                .Where(t => t.CrawlerPrice > 0)
                .Sum(t => t.TotalPrice);

            if (bidding.CrawlerPrice > 0)
                bidding.Score = (int)Math.Round(((bidding.TotalPrice - bidding.CrawlerPrice) / bidding.CrawlerPrice) * 100, 0);
            
            return bidding;
        }

        private async Task CloseLink(CrawlerLink link)
        {
            link.IsBecParsed = true;

            await collections
                .CrawlerLink
                .ReplaceOneAsync(Builders<CrawlerLink>.Filter.Eq(t => t.Id, link.Id), link);
        }
    }
}