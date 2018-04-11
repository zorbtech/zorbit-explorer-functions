using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using Zorbit.Explorer.Functions.Models;
using Zorbit.Explorer.Functions.Properties;
using Zorbit.Explorer.Functions.Utils;

namespace Zorbit.Explorer.Functions
{
    public class GetBlockByHash : ZorbitFunction
    {
        public static async Task<IActionResult> Run(HttpRequestMessage req, CloudTable blockTable, string network, string hash, TraceWriter log)
        {
            if (!ValidNetwork(network))
            {
                return new NotFoundObjectResult(Resources.NetworkNotFound);
            }

            var partitionFilter = TableQuery.GenerateFilterCondition(PartitionKey, QueryComparisons.Equal, network.ToLowerInvariant());

            var keyFilter = string.Empty;
            for (var i = 0; i < 10; i++)
            {
                var currentFilter = TableQuery.GenerateFilterCondition(RowKey, QueryComparisons.Equal, $"{hash}-{i}");
                keyFilter = i == 0 ? currentFilter : TableQuery.CombineFilters(keyFilter, TableOperators.Or, currentFilter);
            }

            var pageQuery = new TableQuery()
            {
                FilterString = TableQuery.CombineFilters(partitionFilter, TableOperators.And, keyFilter),
                TakeCount = 10
            };

            var pageResult = await blockTable.ExecuteQueryAsync(pageQuery);
            var chunks = pageResult.Select(e => new BlockChunkModel(e))
                .ToList();

            if (!chunks.Any())
            {
                return new NotFoundObjectResult(Resources.BlockNotFound);
            }
            
            var block = BlockChunkModel.GetBlock(chunks);
            return new JsonResult(block.ToString());
            //return new JsonPrettyResult(block);
        }
    }
}
