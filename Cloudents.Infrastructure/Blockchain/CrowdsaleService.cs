using System;
using System.Collections.Generic;
using System.Text;
using Cloudents.Core.Interfaces;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexTypes;
using Nethereum.Contracts;


namespace Cloudents.Infrastructure.BlockChain
{
    public class CrowdsaleService : BlockChainProvider, ICrowdSaleService
    {

        
        public CrowdsaleService (IConfigurationKeys configurationKeys) : base (configurationKeys)
        {
        }

        protected override string Abi => "Crowdsale";
      
        protected override string TransactionHash => "0x6296a181d9dc59a31d9ef94e5ff4d0d11f9b709046bc0bc6b8e6ed3bb48632d3";
            //"0xdb64eb3552e0799af575a8980b7a95db34ec9fe657f3a40f78fbf13cd078e0c8";
            //"0x2684cc34acb76754f0d9d267496ccfe6964bfafebf1f5aebe58296e86352fa98";
           

        public async Task<string> BuyTokens(string senderPK, int amount, CancellationToken token)
        {
            var contract = await GetContractAsync(GenerateWeb3Instance(senderPK), token).ConfigureAwait(false);
            var operationToExe = contract.GetFunction("buyTokens");
            HexBigInteger maxGas = new HexBigInteger(4700000);
            HexBigInteger Amount = new HexBigInteger(Convert.ToUInt64(amount) * Convert.ToUInt64(Math.Pow(10, 18)));
            var parameters = (new object[] { });
            
            var receiptFirstAmountSend = await operationToExe.SendTransactionAndWaitForReceiptAsync(GetPublicAddress(senderPK), maxGas, Amount, null, parameters).ConfigureAwait(false);
            
            return receiptFirstAmountSend.ToString();
        }
    }
}
