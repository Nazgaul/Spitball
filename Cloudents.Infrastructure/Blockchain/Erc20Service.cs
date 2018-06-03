using Cloudents.Core.Interfaces;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Infrastructure.BlockChain;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Blockchain
{
    [UsedImplicitly]
    public class Erc20Service : BlockChainProvider, IBlockChainErc20Service
    {
        protected override string Abi => "TokenAbi";

        protected override string TransactionHash => "0x430fdc71d7b86f432ae0d22d0cc11ce7909f0434942f5943f2288f3140dac07d";
            

        public Erc20Service (IConfigurationKeys configurationKeys) : base(configurationKeys)
        {
        }

        public async Task<decimal> GetBalanceAsync(string senderAddress, CancellationToken token)
        {
            var contract = await GetContractAsync(GenerateWeb3Instance(),  token).ConfigureAwait(false);
            var function = contract.GetFunction("balanceOf");
            var result = await function.CallAsync<BigInteger>(senderAddress).ConfigureAwait(false);
            var normalAmount = result / new BigInteger(FromWei);
            return (decimal)normalAmount;
        }

        public async Task<string> TransferMoneyAsync(string senderPk, string toAddress, float amount, CancellationToken token)
        {
          
            var contract = await GetContractAsync(GenerateWeb3Instance(senderPk), token).ConfigureAwait(false);
            var operationToExe = contract.GetFunction("transfer");
            var amountTransformed = new BigInteger(amount * FromWei);
            var receiptFirstAmountSend = await operationToExe.SendTransactionAndWaitForReceiptAsync(GetPublicAddress(senderPk), 70000, token, toAddress, amountTransformed).ConfigureAwait(false);
            return receiptFirstAmountSend.BlockHash;
        }

        public async Task SetInitialBalanceAsync(string address, CancellationToken token)
        {
            await TransferMoneyAsync(SpitballPrivateKey, address, 10, token).ConfigureAwait(false);
        }

    }
}
