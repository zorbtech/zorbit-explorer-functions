using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using Zorbit.Explorer.Functions.Entities;
using Zorbit.Explorer.Functions.Properties;
using Zorbit.Explorer.Functions.Utils;

namespace Zorbit.Explorer.Functions
{
    public class GetHeight : ZorbitFunction
    {
        public static IActionResult Run(HttpRequestMessage req, CloudTable chainTable, string network, TraceWriter log)
        {
            if (!ValidNetwork(network))
            {
                return new NotFoundObjectResult(Resources.NetworkNotFound);
            }

            var tip = chainTable.ExecuteQuery(new TableQuery { TakeCount = 1 })
                .Select(e => new ChainHeaderModel(e))
                .FirstOrDefault();

            if (tip == null)
            {
                return new NotFoundObjectResult("Chain is empty");
            }

            return new JsonPrettyResult(tip.ChainOffset);
        }
    }
}
