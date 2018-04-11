using System;
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
    public class GetBlocks : ZorbitFunction
    {
        public static async Task<IActionResult> Run(HttpRequestMessage req, CloudTable summaryTable, string network, int take, int skip, TraceWriter log)
        {
            if (!ValidNetwork(network))
            {
                return new NotFoundObjectResult(Resources.NetworkNotFound);
            }

            var tip = GetTableHeight(summaryTable);
            if (tip == null)
            {
                return new NotFoundResult();
            }

            var fromOffset = Math.Max(tip.Height - skip, 0);
            var toOffset = Math.Max(fromOffset - (take - 1), 0);

            var from = RowKeyHelper.HeightToString(fromOffset);
            var to = RowKeyHelper.HeightToString(toOffset);

            var partitionFilter = TableQuery.GenerateFilterCondition(PartitionKey, QueryComparisons.Equal, network.ToLowerInvariant());

            var pageFilter = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition(RowKey, QueryComparisons.GreaterThanOrEqual, from), TableOperators.And,
                TableQuery.GenerateFilterCondition(RowKey, QueryComparisons.LessThanOrEqual, to));

            var pageQuery = new TableQuery()
            {
                FilterString = TableQuery.CombineFilters(partitionFilter, TableOperators.And, pageFilter),
                TakeCount = take
            };

            var pageResult = await summaryTable.ExecuteQueryAsync(pageQuery);
            var results = pageResult.Select(e => new BlockSummaryModel(e))
                .ToList();

            return new JsonPrettyResult(results);
        }

        private static BlockSummaryModel GetTableHeight(CloudTable summaryTable)
        {
            return summaryTable.ExecuteQuery(new TableQuery { TakeCount = 1 })
                .Select(e => new BlockSummaryModel(e))
                .FirstOrDefault();
        }
    }
}
