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
    public class ListCrawler
    {
        private MongoCollections collections;

        public ListCrawler()
        {
            this.collections = new MongoCollections();
        }

        public async Task ParseLinks(string htmlToParse)
        {
             var maxSelect = 421;
             var initialSelect = 0;
             var viewState = File.ReadAllText($"viewstate.txt");
             var eventTarget = "ctl00$c_area_conteudo$grdvOC_publico";
             var eventArgument = "Selecionar_OC$";

             for (initialSelect = 0; initialSelect < maxSelect; initialSelect++)
             {
                 var ocLink = await HtmlGetter.PostBodyFrom(
                     url: "http://www.bec.sp.gov.br/BEC_Dispensa_UI/ui/BEC_DL_Pesquisa.aspx?chave=",
                     viewState: viewState,
                     eventTarget: eventTarget,
                     eventArgument: $"{eventArgument}{initialSelect}"
                 );

                 var form = TagGetter.GetAttrFromTag(
                     body: ocLink,
                     tagName: "form",
                     id: "aspnetForm",
                     tagToGet: "action"
                 );
             }
        }

        private async Task SaveLink(CrawlerLink link)
        {
            await collections
                .CrawlerLink
                .InsertOneAsync(link);
        }
    }
}