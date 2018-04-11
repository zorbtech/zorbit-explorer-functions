using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;
using NBitcoin;

namespace Zorbit.Explorer.Functions.Models
{
    public class BlockChunkModel
    {
        public int Index { get; set; }

        public List<byte[]> Chunks { get; }

        public BlockChunkModel(DynamicTableEntity entity)
        {
            Chunks = new List<byte[]>();
            foreach (var property in entity.Properties)
            {
                if (property.Key.Equals("index"))
                {
                    Index = property.Value.Int32Value.Value;
                }
                else if (property.Key.StartsWith("chunk"))
                {
                    Chunks.Add(property.Value.BinaryValue);
                }
            }
        }

        public static Block GetBlock(IEnumerable<BlockChunkModel> entries)
        {
            IEnumerable<byte> bytes = new List<byte>();
            bytes = entries.SelectMany(entry => entry.Chunks)
                .Aggregate(bytes, (current, chunk) => current.Concat(chunk));
            return new Block(bytes.ToArray());
        }
    }
}
