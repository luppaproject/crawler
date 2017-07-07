using System.Threading.Tasks;
using Luppa.Helpers;
using Luppa.Data;
using HtmlAgilityPack;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Luppa.Services.BECList
{
    public class BECListCrawler
    {
        private readonly IMongoCollections collections;

        private const string LIST_PAGE = "http://www.bec.sp.gov.br/BEC_Dispensa_UI/ui/BEC_DL_Pesquisa.aspx?chave=";

        public BECListCrawler(IMongoCollections collections)
        {
            this.collections = collections;
        }

        public async Task StartListCrawler()
        {
             // The magic start here. Our crawler goes to bec's website and gets all tokens and stuff there :)
             Console.ForegroundColor = ConsoleColor.Cyan;
             Console.WriteLine("1/4 - Getting basic structure...");

             var becListVM = await GetInitialViewState();

             Console.WriteLine("2/4 - Now it's time to change params and states ¬¬");
             await ChangeStateAndSetParams(becListVM);

             Console.WriteLine("3/4 - Okie, time to parse some links.");
             var links = await CreateHtmlLinks(becListVM);

             Console.ForegroundColor = ConsoleColor.Cyan;
             Console.WriteLine("4/4 - Easy part: save on database ;)");
        }

        private async Task<List<string>> CreateHtmlLinks(BECListViewStateModel becListVM)
        {
            var document = new HtmlDocument();
            
            document.LoadHtml(becListVM.FinalBodyString);

            // Get all links and remove dirty -.- ...
            var biddingCodeList = document
                .DocumentNode
                .SelectNodes("//table")
                .Where(table => table.Id == "ctl00_c_area_conteudo_grdvOC_publico")
                .First()
                    .SelectNodes("//tr/td/a")
                    .Select(a => a
                        .Attributes["href"]
                        .Value
                        .Split(',')[1]
                        .Replace("'", "")
                        .Replace(")", "")
                    );

            // Well, go take a cofee :)
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            System.Console.WriteLine("Hey, what about drinking coffee. It may take a while ;)");

            var links = new List<string>();

            becListVM.EventTarget = "ctl00$c_area_conteudo$grdvOC_publico";
            // becListVM.AddicionalData.Remove("ctl00$c_area_conteudo$bt33022_Pesquisa");

            foreach (var biddingCode in biddingCodeList)
            {
                becListVM.EventArgument = biddingCode;

                links.Add(
                    await HtmlGetter.PostAndGetRedirectUrl(
                        url: LIST_PAGE,
                        formUrlContent: becListVM.GenerateFormData()
                    )
                );
            }

            return links;
        }

        private async Task<BECListViewStateModel> GetInitialViewState()
        {
            var firstBecPage = await HtmlGetter.GetBodyFrom(LIST_PAGE);
            var document = new HtmlDocument();
            var becListVM = new BECListViewStateModel();

            document.LoadHtml(firstBecPage);

            SetBasicAttributes(document, becListVM);
            
            return becListVM;
        }

        private void SetBasicAttributes(HtmlDocument document, BECListViewStateModel becListVM)
        {
            becListVM.ViewState = document
                .DocumentNode
                .SelectNodes("//input")
                .Where(input => input.Id == "__VIEWSTATE")
                .First()
                .Attributes["value"]
                .Value;

            becListVM.ViewStateGenerator = document
                .DocumentNode
                .SelectNodes("//input")
                .Where(input => input.Id == "__VIEWSTATEGENERATOR")
                .First()
                .Attributes["value"]
                .Value;

            becListVM.EventValidation = document
                .DocumentNode
                .SelectNodes("//input")
                .Where(input => input.Id == "__EVENTVALIDATION")
                .First()
                .Attributes["value"]
                .Value;
        }

        private async Task ChangeStateAndSetParams(BECListViewStateModel becListVM)
        {
            becListVM.EventTarget = "ctl00$c_area_conteudo$Wuc_filtroPesquisaOc1$c_ddlListaAtividadeGrupo";

            becListVM.AddicionalData.Add("ctl00$c_area_conteudo$txtUsuario", " - ");
            becListVM.AddicionalData.Add("ctl00$c_area_conteudo$Wuc_filtroPesquisaOc1$c_ddlListaAtividadeGrupo", "0");
            becListVM.AddicionalData.Add("ctl00$c_area_conteudo$Wuc_filtroPesquisaOc1$c_ddlListaSituacao", "0");
            becListVM.AddicionalData.Add("ctl00$c_area_conteudo$Wuc_filtroPesquisaOc1$cSecretaria", "");
            becListVM.AddicionalData.Add("ctl00$c_area_conteudo$Wuc_filtroPesquisaOc1$cUgeCodigo", "Código");
            becListVM.AddicionalData.Add("ctl00$c_area_conteudo$Wuc_filtroPesquisaOc1$cUgeDenominacao", "Denominação");
            becListVM.AddicionalData.Add("ctl00$c_area_conteudo$Wuc_filtroPesquisaOc1$dpl_municipio", "0");
            becListVM.AddicionalData.Add("ctl00$c_area_conteudo$Wuc_filtroPesquisaOc1$c_txt_ItemMaterial", "Código");
            becListVM.AddicionalData.Add("ctl00$c_area_conteudo$Wuc_filtroPesquisaOc1$c_txt_ItemMaterial_desc", "Denominação");
            becListVM.AddicionalData.Add("ctl00$c_area_conteudo$Wuc_filtroPesquisaOc1$c_tbNumeroOc", "");

            var pageJustFinishedBidding = await HtmlGetter.PostBodyFrom(LIST_PAGE, becListVM.GenerateFormData());
            var document = new HtmlDocument();

            document.LoadHtml(pageJustFinishedBidding);

            SetBasicAttributes(document, becListVM);

            becListVM.CT100ToolkitScript = document
                .DocumentNode
                .SelectNodes("//input")
                .Where(input => input.Id == "ctl00_ToolkitScriptManager1_HiddenField")
                .First()
                .Attributes["value"]
                .Value;

            var fromDate = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");
            var toDate = DateTime.Now.ToString("dd/MM/yyyy");
            
            becListVM.AddicionalData["ctl00$c_area_conteudo$Wuc_filtroPesquisaOc1$c_ddlListaSituacao"] = "505";
            becListVM.AddicionalData.Add("ctl00$c_area_conteudo$Wuc_filtroPesquisaOc1$c_tbDataInicial", fromDate);
            becListVM.AddicionalData.Add("ctl00$c_area_conteudo$Wuc_filtroPesquisaOc1$c_tbDataFinal", toDate);
            becListVM.AddicionalData.Add("ctl00$c_area_conteudo$bt33022_Pesquisa", "Pesquisar");

            // It may take some time to complete =)
            pageJustFinishedBidding = await HtmlGetter.PostBodyFrom(LIST_PAGE, becListVM.GenerateFormData());

            // Trick last time -.-
            SetBasicAttributes(document, becListVM);

            becListVM.FinalBodyString = pageJustFinishedBidding;
        }

        private async Task SaveLink(CrawlerLink link)
        {
            await collections
                .CrawlerLink
                .InsertOneAsync(link);
        }
    }
}