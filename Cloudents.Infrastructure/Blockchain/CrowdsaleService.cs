using System;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.Hex.HexTypes;


namespace Cloudents.Infrastructure.BlockChain
{
    public class CrowdSaleService : BlockChainProvider, ICrowdSaleService
    {


        public CrowdSaleService(IConfigurationKeys configurationKeys) : base(configurationKeys)
        {
        }

        protected override string Abi => "Crowdsale";

        protected override string TransactionHash => "0x6296a181d9dc59a31d9ef94e5ff4d0d11f9b709046bc0bc6b8e6ed3bb48632d3";


        public async Task<string> BuyTokensAsync(string senderPrivateKey, int amount, CancellationToken token)
        {
            var contract = await GetContractAsync(GenerateWeb3Instance(senderPrivateKey), token).ConfigureAwait(false);
            var operationToExe = contract.GetFunction("buyTokens");
            HexBigInteger maxGas = new HexBigInteger(4700000);
            var address = GetPublicAddress(senderPrivateKey);
            //var receiptFirstAmountSend = await operationToExe.SendTransactionAndWaitForReceiptAsync(address, maxGas, token);

            HexBigInteger Amount = new HexBigInteger(Convert.ToUInt64(amount) * Convert.ToUInt64(Math.Pow(10, 18)));
            var parameters = (new object[] { });

            var receiptFirstAmountSend = await operationToExe.SendTransactionAndWaitForReceiptAsync(GetPublicAddress(senderPrivateKey), maxGas, Amount, null, parameters).ConfigureAwait(false);

            return receiptFirstAmountSend.ToString();
        }
    }
}
