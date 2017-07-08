using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Luppa.Data;
using Luppa.Helpers;
using MongoDB.Driver;

namespace Luppa.Services.BEC
{
    public class BECScraper
    {
        private readonly IMongoCollections collections;

        private const string BASE_URL = "http://www.bec.sp.gov.br/BEC_Dispensa_UI/ui/";

        public BECScraper(IMongoCollections collections)
        {
            this.collections = collections;
        }

        public async Task StartScraper(IEnumerable<CrawlerLink> urls)
        {
            Bidding bidding;

            Console.ForegroundColor = ConsoleColor.Cyan;

            foreach (var link in urls)
            {
                Console.WriteLine($"Parsing {link.Url}...");

                bidding = await ParseLink(link);

                if (bidding != null)
                {
                    if (bidding.Products.Count > 0)
                        await collections.Bidding.InsertOneAsync(bidding);

                    await CloseLink(link);
                }
            } 
        }

        private async Task<Bidding> ParseLink(CrawlerLink link)
        {
            var biddindBody = await HtmlGetter.GetBodyFrom(link.Url);
            
            if (string.IsNullOrEmpty(biddindBody))
                return new Bidding();

            var document = new HtmlDocument();
            var bidding = new Bidding() { BiddingUrl = link.Url };

            document.LoadHtml(biddindBody);

            bidding.OrderNumber = document
                .DocumentNode
                .SelectNodes("//span")
                .Where(span => span.Id == "ctl00_DetalhesOfertaCompra1_txtOC")?
                .First()
                .InnerText ?? "";

            bidding.Entity = document
                .DocumentNode
                .SelectNodes("//span")
                .Where(span => span.Id == "ctl00_DetalhesOfertaCompra1_txtNomUge")?
                .First()
                .InnerText ?? "";

            bidding.OrderType = "material-de-consumo";

            var itemsUrl = document
                .DocumentNode
                .SelectNodes("//a")
                .Where(a =>
                    a.Attributes.Contains("href") && 
                    a.Attributes["href"]
                        .Value
                        .Contains("BEC_DL_OC_ITEM.aspx?chave")
                )?
                .First()
                .Attributes["href"]
                .Value ?? "";

            var itemsHtmlBody = await HtmlGetter.GetBodyFrom($"{BASE_URL}{itemsUrl}");

            document.LoadHtml(itemsHtmlBody);

            var itemsTable = document
                .DocumentNode
                .SelectNodes("//table")
                .Where(table => table.Id == "ctl00_c_area_conteudo_wuc_OC_ITEM_LANCE_DISPENSA1_dtgItens")
                .First();

            if (itemsTable == null)
                return bidding;

            var rowsOfItems = itemsTable
                .SelectNodes("//tr")
                .Where(tr => 
                    tr.Attributes.Contains("bgcolor") &&
                    !tr.Attributes["bgcolor"]
                        .Value
                        .Contains("#6B696B")
                )
                .ToList();

            foreach (var row in rowsOfItems)
            {
                var columDocument = new HtmlDocument();
                columDocument.LoadHtml(row.InnerHtml);

                var itemColumns = columDocument
                    .DocumentNode
                    .SelectNodes("//td")
                    .ToList();

                var imageCount = itemColumns[2]
                    .SelectNodes("//img");

                if (imageCount != null)
                    continue;

                var product = new Product
                {
                    ProductName = itemColumns[4].InnerText,
                    Quantity = double.Parse(itemColumns[5].InnerText),
                    Price = double.Parse(itemColumns[7].InnerText),
                    Manufacturer = itemColumns[10].InnerText
                };

                product.TotalPrice = product.Quantity * product.Price;
                bidding.Manufacturer = product.Manufacturer;
                bidding.TotalPrice += product.TotalPrice;
                bidding.Quantity += product.Quantity;

                bidding.Products.Add(product);
            }

            if (bidding.Products.Count > 0)
                bidding.ProductAlias = string.Join(", ", bidding.Products);

            return bidding;
        }

        private async Task CloseLink(CrawlerLink link)
        {
            link.IsBecParsed = true;

            await collections
                .CrawlerLink
                .ReplaceOneAsync(
                    Builders<CrawlerLink>
                        .Filter
                        .Eq(t => t.Id, link.Id), 
                    link
                );
        }
    }
}