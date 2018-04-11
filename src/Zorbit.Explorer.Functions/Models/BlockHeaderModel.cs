﻿using NBitcoin;
using NBitcoin.DataEncoders;
using Newtonsoft.Json;

namespace Zorbit.Explorer.Functions.Models
{
    /// <summary>
    /// Data structure for RPC block headers.
    /// <see cref="https://bitcoin.org/en/developer-reference#getblockheader"/>
    /// </summary>
    public class BlockHeaderModel
    {
        /// <summary>
        /// Constructs a RPC BlockHeaderModel from a block header object.
        /// </summary>
        /// <param name="blockHeader">The block header.</param>
        public BlockHeaderModel(BlockHeader blockHeader)
        {
            this.Version = (uint)blockHeader.Version;
            this.PreviousBlockHash = blockHeader.HashPrevBlock.ToString();
            this.MerkleRoot = blockHeader.HashMerkleRoot.ToString();
            this.Time = blockHeader.Time;
            byte[] bytes = this.GetBytes(blockHeader.Bits.ToCompact());
            string encodedBytes = Encoders.Hex.EncodeData(bytes);
            this.Bits = encodedBytes;
            this.Nonce = (int)blockHeader.Nonce;
        }

        /// <summary>
        /// The blocks version number.
        /// </summary>
        [JsonProperty(PropertyName = "version")]
        public uint Version { get; private set; }

        /// <summary>
        /// The merkle root for this block encoded as hex in RPC byte order.
        /// </summary>
        [JsonProperty(PropertyName = "merkleroot")]
        public string MerkleRoot { get; private set; }

        /// <summary>
        /// The nonce which was successful at turning this particular block
        /// into one that could be added to the best block chain.
        /// </summary>
        [JsonProperty(PropertyName = "nonce")]
        public int Nonce { get; private set; }

        /// <summary>
        /// The target threshold this block's header had to pass.
        /// </summary>
        [JsonProperty(PropertyName = "bits")]
        public string Bits { get; private set; }

        /// <summary>
        /// The hash of the header of the previous block,
        /// encoded as hex in RPC byte order.
        /// </summary>
        [JsonProperty(PropertyName = "previousblockhash")]
        public string PreviousBlockHash { get; private set; }

        /// <summary>
        /// The block time in seconds since epoch (Jan 1 1970 GMT).
        /// </summary>
        [JsonProperty(PropertyName = "time")]
        public uint Time { get; private set; }

        /// <summary>
        /// Convert compact of miner challenge to byte format,
        /// serialized for transmission via RPC.
        /// </summary>
        /// <param name="compact">Compact representation of challenge.</param>
        /// <returns>Byte representation of challenge.</returns>
        /// <seealso cref="Target"/>
        private byte[] GetBytes(uint compact)
        {
            return new byte[]
            {
                (byte)(compact >> 24),
                (byte)(compact >> 16),
                (byte)(compact >> 8),
                (byte)(compact)
            };
        }
    }
}