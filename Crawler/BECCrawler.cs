using System.Collections.Generic;
using System.Threading.Tasks;
using Luppa.Data;
using System.Net.Http;
using System.Net;
using System.Xml.Linq;
using System.Linq;
using MongoDB.Driver;

namespace Luppa.Crawler
{
    public class BECCrawler
    {
        private MongoCollections collections;

        public BECCrawler()
        {
            this.collections = new MongoCollections();
        }

        public async Task ParseLinks(List<CrawlerLink> linkList)
        {
            Bidding bidding;

            foreach (var link in linkList)
            {
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

            var bidding = new Bidding() { BiddingUrl = link.Url };
            var tableItems = TagGetter.GetValueFromTag(
                body: biddindBody, 
                tagName: "table", 
                id: "ctl00_c_area_conteudo_wuc_OC_ITEM_LANCE_DISPENSA1_dtgItens"
            );
            var itemsRows = TagGetter.ParseTableRows(tableItems).ToList();
            var itemColumns = new List<string>();

            foreach (var row in itemsRows)
            {
                itemColumns = new List<string>();
                itemColumns.AddRange(TagGetter.ParseTableColumns(row));

                if (itemColumns.Count == 0 || itemColumns[2].Contains("img"))
                    continue;

                var product = new Product
                {
                    ProductName = itemColumns[4],
                    Quantity = double.Parse(itemColumns[5]),
                    Price = double.Parse(itemColumns[7]),
                    Manufacturer = itemColumns[10]
                };

                product.TotalPrice = product.Quantity * product.Price;
                bidding.Manufacturer = product.Manufacturer;
                bidding.TotalPrice += product.TotalPrice;
                bidding.Quantity += product.Quantity;

                bidding.Products.Add(product);
            }

            if (bidding.Products.Count > 0)
                bidding.ProductAlias = string.Join(", ", bidding.Products);

            bidding.OrderNumber = TagGetter.GetValueFromTag(biddindBody, "span", "ctl00_DetalhesOfertaCompra1_txtOC");
            bidding.Entity = TagGetter.GetValueFromTag(biddindBody, "span", "ctl00_DetalhesOfertaCompra1_txtNomUge");
            bidding.OrderType = "material-de-consumo";

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