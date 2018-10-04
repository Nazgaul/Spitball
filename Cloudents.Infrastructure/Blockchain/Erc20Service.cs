using System;
using Cloudents.Core.Interfaces;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Infrastructure.BlockChain;
using JetBrains.Annotations;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Signer;

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

        public async Task<string> TransferPreSigned(string fromPK, string to, int amount, int fee, CancellationToken token)
        {
            Account SpitballAccountt = new Account(SpitballPrivateKey);
            var web3 = new Web3(SpitballAccountt);

            var txCount = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(GetAddress(SpitballPrivateKey));
            var nonce = txCount.Value;
            var amountTransformed = new BigInteger(amount * FromWei);
            var feeTransformed = new BigInteger(fee * FromWei);

            string str = "48664c16"
                        + ContractAddress.RemoveHexPrefix()
                        + to.RemoveHexPrefix()
                        + amountTransformed.ToString("X64")
                        + feeTransformed.ToString("X64")
                        + nonce.ToString("X64");

            var byteStr = HexByteConvertorExtensions.HexToByteArray(str);
            var sha3 = new Nethereum.Util.Sha3Keccack();
            var res = sha3.CalculateHash(byteStr);
            var messageSigner = new MessageSigner();
            var sig = messageSigner.Sign(res, fromPK).HexToByteArray();
           
            var function = await GetFunctionAsync("transferPreSigned", token).ConfigureAwait(false);
            var receiptFirstAmountSend = await function.SendTransactionAndWaitForReceiptAsync(SpitballPrivateKey, MaxGas, token, sig, to, amountTransformed, feeTransformed, nonce).ConfigureAwait(false);
            return receiptFirstAmountSend.BlockHash;
        }


        public async Task<string> ApprovePreSigned(string fromPK, string sender, int amount, int fee, CancellationToken token)
        {
            Account SpitballAccountt = new Account(SpitballPrivateKey);
            var web3 = new Web3(SpitballAccountt);

            var txCount = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(GetAddress(SpitballPrivateKey));
            var nonce = txCount.Value;
            var amountApproved = new BigInteger(amount * FromWei);
            var feeApproved = new BigInteger(fee * FromWei);


            string str = "f7ac9c2e"
                       + ContractAddress.RemoveHexPrefix()
                       + sender.RemoveHexPrefix()
                       + amountApproved.ToString("X64")
                       + feeApproved.ToString("X64")
                       + nonce.ToString("X64");

            var byteStr = HexByteConvertorExtensions.HexToByteArray(str);
            var sha3 = new Nethereum.Util.Sha3Keccack();
            var res = sha3.CalculateHash(byteStr);
            var messageSigner = new MessageSigner();
            var sig = messageSigner.Sign(res, fromPK).HexToByteArray();


            var function = await GetFunctionAsync("approvePreSigned", token).ConfigureAwait(false);
            var receiptFirstAmountSend = await function.SendTransactionAndWaitForReceiptAsync(SpitballPrivateKey, MaxGas, token, sig, sender, amountApproved, feeApproved, nonce).ConfigureAwait(false);
            return receiptFirstAmountSend.BlockHash;
        }

        public async Task<decimal> GetAllowanceAsync([NotNull] string ownerAddress,[NotNull] string spenderAddress, CancellationToken token)
        {
            if (ownerAddress == null) throw new ArgumentNullException(nameof(ownerAddress));
            if (spenderAddress == null) throw new ArgumentNullException(nameof(spenderAddress));

            var function = await GetFunctionAsync("allowance", token).ConfigureAwait(false);
            var result = await function.CallAsync<BigInteger>(ownerAddress, spenderAddress).ConfigureAwait(false);
            var normalAmount = result / new BigInteger(FromWei);
            return (decimal)normalAmount;
        }

        public async Task<string> IncreaseApproval(string spender, int amount, CancellationToken token)
        {

            var function = await GetFunctionAsync("increaseApproval", token).ConfigureAwait(false);
            var amountIncreaseApproved = new BigInteger(amount * FromWei);
            var receiptFirstAmountSend = await function.SendTransactionAndWaitForReceiptAsync(SpitballPrivateKey, MaxGas, token, spender, amountIncreaseApproved).ConfigureAwait(false);
            return receiptFirstAmountSend.BlockHash;

        }
    }
}



