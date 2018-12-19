using System;

namespace Cloudents.Application.Message
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