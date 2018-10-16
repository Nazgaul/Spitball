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
using System.Collections.Generic;
using System.Text;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.RPC.Eth.DTOs;


namespace Cloudents.Infrastructure.Blockchain
{
    [UsedImplicitly]
    public class Erc20Service : BlockChainProvider, IBlockChainErc20Service
    {
        protected override string Abi => "TokenAbi";

        protected override string ContractAddress => "0xc493652412dccaec49698d6f719273f279cfd893";//"0x045904c2a9d1a54f9d1bda40d4b2551ee3f3d9ff";//"0x4848e858f625fa67b8ee765b4d0412587da3dd74";

        public Erc20Service(IConfigurationKeys configurationKeys) : base(configurationKeys)
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

            var receiptFirstAmountSend = await function.SendTransactionAndWaitForReceiptAsync(senderPk, MaxGas, token, toAddress, amountTransformed).ConfigureAwait(false);
            return receiptFirstAmountSend.BlockHash;
        }

        public async Task SetInitialBalanceAsync(string address, CancellationToken token)
        {
            await TransferMoneyAsync(SpitballPrivateKey, address, 100, token).ConfigureAwait(false);
        }

        public async Task<string> MintNewTokens(string toAddress, int amount, CancellationToken token)
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

        public async Task<string> ApproveAsync(string spender, int amount, CancellationToken token)
        {
            var function = await GetFunctionAsync("approve", token).ConfigureAwait(false);
            var amountApproved = new BigInteger(amount * FromWei);
            var receiptFirstAmountSend = await function.SendTransactionAndWaitForReceiptAsync(SpitballPrivateKey, MaxGas, token, spender, amountApproved).ConfigureAwait(false);
            return receiptFirstAmountSend.BlockHash;
        }

        public async Task<string> TransferPreSignedAsync(string delegatePK, string fromPK, string to, int amount, int fee, CancellationToken token)
        {
            Account delegateAccountt = new Account(delegatePK);
            var web3 = new Web3(delegateAccountt);

            var txCount = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(GetAddress(delegatePK));
            
            var nonce = txCount.Value;
            Console.WriteLine($"Address: {GetAddress(delegatePK)}, Nonce: {nonce}");
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
            
            var function = await GetFunctionAsync("transferPreSigned", delegatePK, token).ConfigureAwait(false);
            
            var receiptFirstAmountSend = await function.SendTransactionAndWaitForReceiptAsync(delegatePK, MaxGas, token, sig, to, amountTransformed, feeTransformed, nonce).ConfigureAwait(false);
            var contract = await GetContractAsync(web3, token);
            var bidAddedEventLog = contract.GetEvent("TransferPreSigned");
            var filterInput = bidAddedEventLog.CreateFilterInput(new BlockParameter(receiptFirstAmountSend.BlockNumber), BlockParameter.CreateLatest());
            var logs = await bidAddedEventLog.GetAllChanges<TransferPreSignedDTO>(filterInput);
            return receiptFirstAmountSend.BlockHash;
        }

        //public async Task<string> TransferPreSignedAsync(string fromPK, string to, int amount, int fee, CancellationToken token)
        //{
        //    Account SpitballAccountt = new Account(SpitballPrivateKey);
        //    var web3 = new Web3(SpitballAccountt);

        //    var txCount = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(GetAddress(SpitballPrivateKey));
        //    var nonce = txCount.Value;
        //    var amountTransformed = new BigInteger(amount * FromWei);
        //    var feeTransformed = new BigInteger(fee * FromWei);

        //    string str = "48664c16"
        //                + ContractAddress.RemoveHexPrefix()
        //                + to.RemoveHexPrefix()
        //                + amountTransformed.ToString("X64")
        //                + feeTransformed.ToString("X64")
        //                + nonce.ToString("X64");

        //    var byteStr = HexByteConvertorExtensions.HexToByteArray(str);
        //    var sha3 = new Nethereum.Util.Sha3Keccack();
        //    var res = sha3.CalculateHash(byteStr);
        //    var messageSigner = new MessageSigner();
        //    var sig = messageSigner.Sign(res, fromPK).HexToByteArray();


        //    var function = await GetFunctionAsync("transferPreSigned", token).ConfigureAwait(false);
        //    var receiptFirstAmountSend = await function.SendTransactionAndWaitForReceiptAsync(SpitballPrivateKey, MaxGas, token, sig, to, amountTransformed, feeTransformed, nonce).ConfigureAwait(false);
        //    var contract = await GetContractAsync(web3, token);
        //    var bidAddedEventLog = contract.GetEvent("TransferPreSigned");
        //    var filterInput =
        //       bidAddedEventLog.CreateFilterInput(new BlockParameter(receiptFirstAmountSend.BlockNumber), BlockParameter.CreateLatest());
        //    var logs = await bidAddedEventLog.GetAllChanges<TransferPreSignedDTO>(filterInput);
        //    return receiptFirstAmountSend.BlockHash;
        //}

        /*public async Task<string> TransferPreSignedAsync(string fromPK, string to, int amount, int fee, double gasPrice, BigInteger nonce, CancellationToken token)
        {
            Account SpitballAccountt = new Account(SpitballPrivateKey);
            var web3 = new Web3(SpitballAccountt);

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
            var receiptFirstAmountSend = await function.SendTransactionAndWaitForReceiptAsync(SpitballPrivateKey, MaxGas, gasPrice, token, sig, to, amountTransformed, feeTransformed, nonce).ConfigureAwait(false);

            return receiptFirstAmountSend.BlockHash;
        }*/


        public async Task<string> ApprovePreSignedAsync(string fromPK, string sender, int amount, int fee, CancellationToken token)
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

        public async Task<decimal> GetAllowanceAsync([NotNull] string ownerAddress, [NotNull] string spenderAddress, CancellationToken token)
        {
            if (ownerAddress == null) throw new ArgumentNullException(nameof(ownerAddress));
            if (spenderAddress == null) throw new ArgumentNullException(nameof(spenderAddress));

            var function = await GetFunctionAsync("allowance", token).ConfigureAwait(false);
            var result = await function.CallAsync<BigInteger>(ownerAddress, spenderAddress).ConfigureAwait(false);
            var normalAmount = result / new BigInteger(FromWei);
            return (decimal)normalAmount;
        }

        public async Task<string> IncreaseApprovalAsync(string spender, int amount, CancellationToken token)
        {

            var function = await GetFunctionAsync("increaseApproval", token).ConfigureAwait(false);
            var amountIncreaseApproved = new BigInteger(amount * FromWei);
            var receiptFirstAmountSend = await function.SendTransactionAndWaitForReceiptAsync(SpitballPrivateKey, MaxGas, token, spender, amountIncreaseApproved).ConfigureAwait(false);
            return receiptFirstAmountSend.BlockHash;

        }

        public async Task<string> IncreaseApprovalPreSignedAsync(string fromPK, string sender, int amount, int fee, CancellationToken token)
        {
            Account SpitballAccountt = new Account(SpitballPrivateKey);
            var web3 = new Web3(SpitballAccountt);

            var txCount = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(GetAddress(SpitballPrivateKey));
            var nonce = txCount.Value + 1;
            var amountApproved = new BigInteger(amount * FromWei);
            var feeApproved = new BigInteger(fee * FromWei);


            string str = "a45f71ff"
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


            var function = await GetFunctionAsync("increaseApprovalPreSigned", token).ConfigureAwait(false);
            var receiptFirstAmountSend = await function.SendTransactionAndWaitForReceiptAsync(SpitballPrivateKey, MaxGas, GasPrice, token, sig, sender, amountApproved, feeApproved, nonce).ConfigureAwait(false);
            return receiptFirstAmountSend.BlockHash;
        }

        public async Task<string> WhitelistUserForTransfers(string userAddress, CancellationToken token)
        {
            Account SpitballAccountt = new Account(SpitballPrivateKey);
            var web3 = new Web3(SpitballAccountt);
            var function = await GetFunctionAsync("whitelistUserForTransfers", token).ConfigureAwait(false);
            var receiptFirstAmountSend = await function.SendTransactionAndWaitForReceiptAsync(SpitballPrivateKey, MaxGas, GasPrice, token, userAddress).ConfigureAwait(false);
            return receiptFirstAmountSend.BlockHash;
        }
    }

    public class TransferPreSignedDTO
    {
        [Parameter("address", "from", 1, true)]
        public string from { get; set; }
        [Parameter("address", "to", 2, true)]
        public string to { get; set; }
        [Parameter("address", "delegate", 3, true)]
        public string delegateAddress { get; set; }
        [Parameter("uint256", "amount", 4, false)]
        public BigInteger amount { get; set; }
        [Parameter("uint256", "fee", 5, false)]
        public BigInteger fee { get; set; }
    }

        
}
