﻿using System;
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
      
        protected override string TransactionHash => "0x895b8f191a3f6c827ae7d2a7b364f00467ed16a1a48e661c2879c5e869b6e884";
        //"0xdb64eb3552e0799af575a8980b7a95db34ec9fe657f3a40f78fbf13cd078e0c8";
        //"0x2684cc34acb76754f0d9d267496ccfe6964bfafebf1f5aebe58296e86352fa98";
        protected override string ContractAddress => "0x26f54bb3efd88b3fe581d4a6ff45456716159a55";


        public async Task<string> BuyTokensAsync(string senderPK, int amount, CancellationToken token)
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
