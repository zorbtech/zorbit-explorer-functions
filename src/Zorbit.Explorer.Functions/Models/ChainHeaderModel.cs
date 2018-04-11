using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;
using NBitcoin;

namespace Zorbit.Explorer.Functions.Entities
{
    public class ChainHeaderModel
    {
        public int ChainOffset { get; }

        public List<BlockHeader> BlockHeaders { get; }

        public ChainHeaderModel(DynamicTableEntity entity)
        {
            ChainOffset = RowKeyHelper.StringToHeight(entity.RowKey);
            BlockHeaders = new List<BlockHeader>();

            foreach (var prop in entity.Properties)
            {
                var header = new BlockHeader();
                header.FromBytes(prop.Value.BinaryValue);
                BlockHeaders.Add(header);
            }
        }
    }
}
