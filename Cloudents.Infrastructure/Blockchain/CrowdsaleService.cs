using System;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.Hex.HexTypes;


namespace Cloudents.Infrastructure.BlockChain
{
    public class CrowdSaleService : BlockChainProvider, IBlockChainCrowdSaleService
    {


        public CrowdSaleService(IConfigurationKeys configurationKeys) : base(configurationKeys)
        {
        }

        protected override string Abi => "Crowdsale";
        private const double Gas = 3e6;
        protected override string TransactionHash => "0x895b8f191a3f6c827ae7d2a7b364f00467ed16a1a48e661c2879c5e869b6e884";
    
        protected override string ContractAddress => "0xf399599acd6bbef4975a3d17eb6743ca8f50baec";


        public async Task<string> BuyTokensAsync(string senderPrivateKey, int amount, CancellationToken token)
        {
            var function = await GetFunctionAsync("buyTokens", token).ConfigureAwait(false);
            
            
            HexBigInteger maxGas = new HexBigInteger(4700000);
            var address = GetPublicAddress(senderPrivateKey);
            
            HexBigInteger Amount = new HexBigInteger(Convert.ToUInt64(amount) * Convert.ToUInt64(Math.Pow(10, 18)));
            var parameters = (new object[] { });
            var receiptFirstAmountSend = await function.SendTransactionAndWaitForReceiptAsync(GetPublicAddress(senderPrivateKey), maxGas, Amount, null, parameters).ConfigureAwait(false);
            return receiptFirstAmountSend.ToString();
        }

        public async Task Withdrawal(CancellationToken token)
        {
            HexBigInteger maxGas = new HexBigInteger(4700000);
            var function = await GetFunctionAsync("safeWithdrawal", token).ConfigureAwait(false);
            var receiptFirstAmountSend = await function.SendTransactionAndWaitForReceiptAsync(GetPublicAddress(SpitballPrivateKey), maxGas,  null).ConfigureAwait(false);
        }
    }
}
