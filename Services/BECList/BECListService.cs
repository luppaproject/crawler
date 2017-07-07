using System;
using System.Threading.Tasks;
using Luppa.Data;
using Luppa.Services.Common;

namespace Luppa.Services.BECList
{
    public class BECListService : ILuppaService
    {
        public async Task StartService()
        {
            var becListCrawler = new BECListCrawler(new MongoCollections());

            await becListCrawler.StartListCrawler();
        }
    }
}