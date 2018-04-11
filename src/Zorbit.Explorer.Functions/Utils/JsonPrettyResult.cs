using System.Net;
using Microsoft.AspNetCore.Mvc;
using NBitcoin.JsonConverters;

namespace Zorbit.Explorer.Functions
{
    public class JsonPrettyResult : ContentResult
    {
        public JsonPrettyResult(object result)
        {
            Content = Serializer.ToString(result);
            ContentType = "application/json";
            StatusCode = (int)HttpStatusCode.OK;
        }
    }
}
