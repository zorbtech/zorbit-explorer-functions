using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using Zorbit.Explorer.Functions.Entities;
using Zorbit.Explorer.Functions.Models;
using Zorbit.Explorer.Functions.Properties;
using Zorbit.Explorer.Functions.Utils;

namespace Zorbit.Explorer.Functions
{
    public class GetBlockByHeight : ZorbitFunction
    {
        public static IActionResult Run(HttpRequestMessage req, CloudTable chainTable, string network, int height, TraceWriter log)
        {
            if (!ValidNetwork(network))
            {
                return new NotFoundObjectResult(Resources.NetworkNotFound);
            }

            var rowKey = RowKeyHelper.HeightToString(height);

            var filter = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition(PartitionKey, QueryComparisons.Equal, network.ToLowerInvariant()), 
                TableOperators.And,
                TableQuery.GenerateFilterCondition(RowKey, QueryComparisons.Equal, rowKey));

            var tableQuery = new TableQuery()
            {
                FilterString = filter,
                TakeCount = 1
            };
            
            var results = chainTable.ExecuteQuery(tableQuery);
            var chainHeader = results.Select(e => new ChainHeaderModel(e)).FirstOrDefault();

            if (chainHeader == null)
            {
                return new NotFoundObjectResult(Resources.BlockNotFound);
            }

            var block = chainHeader.BlockHeaders.Last();

            return new JsonPrettyResult(new BlockHeaderModel(block));
        }
    }
}
