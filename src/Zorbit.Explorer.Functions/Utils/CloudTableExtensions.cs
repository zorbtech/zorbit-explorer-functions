using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.WindowsAzure.Storage.Table;

namespace Zorbit.Explorer.Functions
{
    public static class CloudTableExtensions
    {
        public static IEnumerable<DynamicTableEntity> ExecuteQuery(this CloudTable table, TableQuery query, CancellationToken ct = default(CancellationToken))
        {
            TableContinuationToken token = null;

            do
            {
                var seg = table.ExecuteQuerySegmentedAsync(query, token).GetAwaiter().GetResult();
                token = seg.ContinuationToken;
                foreach (var tableEntity in seg)
                    yield return tableEntity;
            } while (token != null && !ct.IsCancellationRequested);
        }

        public static async System.Threading.Tasks.Task<IEnumerable<DynamicTableEntity>> ExecuteQueryAsync(this CloudTable table, TableQuery query)
        {
            TableContinuationToken token = null;
            var retVal = new List<DynamicTableEntity>();
            do
            {
                var results = await table.ExecuteQuerySegmentedAsync(query, token);
                retVal.AddRange(results.Results);
                token = results.ContinuationToken;
            } while (token != null);

            return retVal;
        }

        public static async System.Threading.Tasks.Task<IEnumerable<T>> ExecuteQueryAsync<T>(this CloudTable table, TableQuery<T> query) where T : ITableEntity, new()
        {
            TableContinuationToken token = null;
            var retVal = new List<T>();
            do
            {
                var results = await table.ExecuteQuerySegmentedAsync(query, token);
                retVal.AddRange(results.Results);
                token = results.ContinuationToken;
            } while (token != null);

            return retVal;
        }
    }
}
