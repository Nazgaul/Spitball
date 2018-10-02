using System;
using Cloudents.Core.Interfaces;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Infrastructure.BlockChain;
using JetBrains.Annotations;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3;

namespace Cloudents.Infrastructure.Blockchain
{
    [UsedImplicitly]
    public class Erc20Service : BlockChainProvider, IBlockChainErc20Service
    {
        protected override string Abi => "TokenAbi";

        protected override string ContractAddress => "0x4848e858f625fa67b8ee765b4d0412587da3dd74";

        public Erc20Service (IConfigurationKeys configurationKeys) : base(configurationKeys)
        {
        }

        public async Task<decimal> GetBalanceAsync([NotNull] string senderAddress, CancellationToken token)
        {
            if (senderAddress == null) throw new ArgumentNullException(nameof(senderAddress));
            var function = await GetFunctionAsync("balanceOf", token).ConfigureAwait(false);
            var result = await function.CallAsync<BigInteger>(senderAddress).ConfigureAwait(false);
            var normalAmount = result / new BigInteger(FromWei);
            //return (decimal)result;
            return (decimal)normalAmount;
        }

        public async Task<string> TransferMoneyAsync(string senderPk, string toAddress, float amount, CancellationToken token)
        {
            var function = await GetFunctionAsync("transfer", token).ConfigureAwait(false);
            var amountTransformed = new BigInteger(amount * FromWei);

            var publicAddress =
              Web3.GetAddressFromPrivateKey(senderPk);
            
            //var gas = await function.EstimateGasAsync(publicAddress, null, null, toAddress, amount);

            var receiptFirstAmountSend = await function.SendTransactionAndWaitForReceiptAsync(senderPk, 4000000, token, toAddress, amountTransformed).ConfigureAwait(false);
            return receiptFirstAmountSend.BlockHash;
        }

        public async Task SetInitialBalanceAsync(string address, CancellationToken token)
        {
            await TransferMoneyAsync(SpitballPrivateKey, address, 100, token).ConfigureAwait(false);
        }

        public async Task<string> CreateNewTokens(string toAddress, int amount, CancellationToken token)
        {
            var function = await GetFunctionAsync("mint", token).ConfigureAwait(false);
            var amountTransformed = new BigInteger(amount * FromWei);

            var receiptFirstAmountSend = await function.SendTransactionAndWaitForReceiptAsync(SpitballPrivateKey, MaxGas, token, toAddress, amountTransformed).ConfigureAwait(false);
            return receiptFirstAmountSend.BlockHash;
        }

        public string GetAddress([NotNull] string privateKey)
        {
            if (privateKey == null) throw new ArgumentNullException(nameof(privateKey));
            return Web3.GetAddressFromPrivateKey(privateKey);
        }

        public (string privateKey, string publicAddress) CreateAccount()
        {
            var ecKey = Nethereum.Signer.EthECKey.GenerateKey();
            var privateKey = ecKey.GetPrivateKeyAsBytes();
            var address = Web3.GetAddressFromPrivateKey(privateKey.ToHex());
            return (privateKey.ToHex(), address);
        }

        public async Task<string> Approve(string spender, int amount, CancellationToken token)
        {
            var function = await GetFunctionAsync("approve", token).ConfigureAwait(false);
            var amountApproved = new BigInteger(amount * FromWei);
            var receiptFirstAmountSend = await function.SendTransactionAndWaitForReceiptAsync(SpitballPrivateKey, MaxGas, token, spender, amountApproved).ConfigureAwait(false);
            return receiptFirstAmountSend.BlockHash;
        }

       /* public async Task<string> ApprovePreSigned(string sig, string spender, int amount, int fee, int nonce)
        {

        }*/
    }
}
