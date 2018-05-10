using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.HdWallet;
using System.Threading;
using Nethereum.Hex.HexTypes;

namespace Cloudents.Core.Interfaces
{
    public interface IBlockchainProvider
    {

        Task<long> GetBalanceAsync(string networkUrl, string accountAddress);
        string CreateAccount();
        Task<int> SendTxAsync(string senderAddress, string senderPK, string recipientAddress, string azureUrl);
        Task<T> MessageContractAsync<T>(string contractHash, string azureUrl, string abi, string functionName);
       // Task<string> TxContractTxContractAsync(string operation, string senderAddress, string senderPK, string contractHash, string abi, string azureUrl);
        string GetPublicAddress(string privateKey); //Get Public from from privete

        //return privete to user
    }
}
