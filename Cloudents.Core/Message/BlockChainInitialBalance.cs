using System;

namespace Cloudents.Core.Message
{
    [Serializable]
    public class BlockChainInitialBalance
    {
        public BlockChainInitialBalance(string publicAddress)
        {
            PublicAddress = publicAddress;
        }

        public string PublicAddress { get; set; }
    }
}