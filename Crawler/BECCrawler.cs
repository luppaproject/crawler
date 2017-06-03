using System.Collections.Generic;
using System.Threading.Tasks;
using Luppa.Data;
using System.Net.Http;
using System.Net;
using System.Xml.Linq;
using System.Linq;

namespace Luppa.Crawler
{
    public class BECCrawler
    {
        public async Task ParseLinks(List<CrawlerLink> linkList)
        {
            Bidding bidding;

            foreach (var link in linkList)
            {
                bidding = await ParseLink(link);

                if (bidding != null)
                    await CloseLink(link);
            }    
        }

        private async Task<Bidding> ParseLink(CrawlerLink link)
        {
            var biddindBody = await HtmlGetter.GetBodyFrom(link.Url);
            
            if (string.IsNullOrEmpty(biddindBody))
                return new Bidding();

            var bidding = new Bidding();
            var tableItems = TagGetter.GetValueFromTag(
                body: biddindBody, 
                tagName: "table", 
                id: "ctl00_c_area_conteudo_wuc_OC_ITEM_LANCE_DISPENSA1_dtgItens"
            );
            var itensTr = TagGetter.GetValueFromTag(
                body: tableItems, 
                tagName: "tr",
                query: "style:\"background-color:#F7F7DE;\""
            );
            var itens = TagGetter.ParseTableColumns(itensTr).ToList();

            bidding.OrderNumber = TagGetter.GetValueFromTag(biddindBody, "span", "ctl00_DetalhesOfertaCompra1_txtOC");
            bidding.OrderType = "MATERIAL DE CONSUMO";
            bidding.ProductName = itens[4];
            bidding.Quantity = decimal.Parse(itens[5]);
            bidding.Price = decimal.Parse(itens[7]);
            bidding.Manufacturer = itens[10];

            return bidding;
        }

        private async Task CloseLink(CrawlerLink link)
        {
            await Task.FromResult(new {});
        }
    }
}