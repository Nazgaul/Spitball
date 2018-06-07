using System;
using System.Numerics;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Nethereum.Hex.HexTypes;

namespace Cloudents.Infrastructure.BlockChain
{
    [UsedImplicitly]
    public class CrowdSaleService : BlockChainProvider, IBlockChainCrowdSaleService
    {
        public CrowdSaleService(IConfigurationKeys configurationKeys) : base(configurationKeys)
        {
        }

        protected override string Abi => "CrowdSale";

        protected override string ContractAddress => "0x995883f8461682382dafeb87577254fb8c1a1e2e";

        public async Task<string> BuyTokensAsync(string senderPrivateKey, int amount, CancellationToken token)
        {
            var function = await GetFunctionAsync("buyTokens", token).ConfigureAwait(false);
            var maxGas = new HexBigInteger(4700000);
            var amountBigInteger = new HexBigInteger(new BigInteger(amount * FromWei));
            var parameters = Array.Empty<object>();
            var receiptFirstAmountSend = await function.SendTransactionAndWaitForReceiptAsync(GetPublicAddress(senderPrivateKey), maxGas, amountBigInteger, null, parameters).ConfigureAwait(false);
            return receiptFirstAmountSend.ToString();
        }

        public async Task Withdrawal(CancellationToken token)
        {
            var maxGas = new HexBigInteger(4700000);
            var function = await GetFunctionAsync("safeWithdrawal", token).ConfigureAwait(false);
            var receiptFirstAmountSend = await function.SendTransactionAndWaitForReceiptAsync(GetPublicAddress(SpitballPrivateKey), maxGas,  null).ConfigureAwait(false);
        }
    }
}
