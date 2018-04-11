using System;
using Microsoft.WindowsAzure.Storage.Table;
using NBitcoin;
using Newtonsoft.Json;

namespace Zorbit.Explorer.Functions.Models
{
    public class BlockSummaryModel
    {
        [JsonProperty(PropertyName = "height")]
        public int Height { get; set; }

        [JsonProperty(PropertyName = "hash")]
        public string Hash { get; set; }

        [JsonProperty(PropertyName = "time")]
        public long Time { get; set; }

        [JsonProperty(PropertyName = "txcount")]
        public int TxCount { get; set; }

        [JsonProperty(PropertyName = "txtotal")]
        public long TxTotal { get; set; }

        [JsonProperty(PropertyName = "size")]
        public int Size { get; set; }

        [JsonProperty(PropertyName = "posblock")]
        public bool PosBlock { get; set; }

        public BlockSummaryModel(DynamicTableEntity entity)
        {
            Height = RowKeyHelper.StringToHeight(entity.RowKey);
            Hash = entity.Properties["hash"].StringValue;

            var time = entity.Properties["time"].DateTimeOffsetValue;
            if (time != null)
            {
                Time = Utils.DateTimeToUnixTime(time.Value);
            }

            var txcount = entity.Properties["txcount"].Int32Value;
            if (txcount != null)
            {
                TxCount = txcount.Value;

            }

            var txtotal = entity.Properties["txtotal"].Int64Value;
            if (txtotal != null)
            {
                TxTotal = Money.Satoshis(txtotal.Value);
            }

            var size = entity.Properties["size"].Int32Value;
            if (size != null)
            {
                Size = size.Value;
            }

            var posBlock = entity.Properties["pos"].BooleanValue;
            if (posBlock != null)
            {
                PosBlock = posBlock.Value;
            }
        }
    }
}
