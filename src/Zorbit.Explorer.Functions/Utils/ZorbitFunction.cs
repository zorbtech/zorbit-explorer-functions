using NBitcoin;

namespace Zorbit.Explorer.Functions.Utils
{
    public abstract class ZorbitFunction
    {
        public const string PartitionKey = "PartitionKey";
        public const string RowKey = "RowKey";
        protected static bool ValidNetwork(string network)
        {
            var valid = false;
            if (network.ToLowerInvariant().Equals("stratistest"))
            {
                valid = Network.StratisTest != null;
            }
            else if (network.ToLowerInvariant().Equals("stratismain"))
            {
                valid = Network.StratisMain != null;
            }

            return valid;
        }
    }
}
