using Cloudents.Core.Interfaces;
using System;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexTypes;
using Nethereum.Contracts;

namespace Cloudents.Infrastructure.BlockChain
{
    public class BlockChainProvider : IBlockChainProvider
    {
        private readonly IConfigurationKeys _configurationKeys;

        private const double FromWei = 1e18;

        public BlockChainProvider(IConfigurationKeys configurationKeys)
        {
            _configurationKeys = configurationKeys;
        }

        private Web3 GenerateWeb3Instance(string senderPk = null)
        {
            if (senderPk != null)
            {
                var account = new Account(senderPk);
                var web3 = new Web3(account, _configurationKeys.BlockChainNetwork);
                return web3;
            }
            else
            {
                var web3 = new Web3(_configurationKeys.BlockChainNetwork);
                return web3;
            }
        }

        private static async Task<Contract> GetContractAsync(Web3 web3, CancellationToken token)
        {
            // "0xa09db301ad49fb1e240f7fe6c4a70edadd9506d93278fb412b571cf8b2786aa4"; //old ICO Contract Hash
            const string transactionHash = "0x175bcd364676ab6632be0ef862723a09370b206c7f9ea7c9ef79cf0889fad8b1"; //ICO Contract Hash
                                                                                                                 //0x175bcd364676ab6632be0ef862723a09370b206c7f9ea7c9ef79cf0889fad8b1

            var deploymentReceipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash).ConfigureAwait(false);
            while (deploymentReceipt == null)
            {
                await Task.Delay(TimeSpan.FromSeconds(0.5), token).ConfigureAwait(false);
                deploymentReceipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash).ConfigureAwait(false);
            }
            var contractAddress = deploymentReceipt.ContractAddress;
            var abi = await ReadApiAsync(token).ConfigureAwait(false);
            return web3.Eth.GetContract(abi, contractAddress);
        }

        private static string _abiContract;

        private static async Task<string> ReadApiAsync(CancellationToken token)
        {
            if (_abiContract == null)
            {
                using (var stream = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("Cloudents.Infrastructure.BlockChain.abi.json"))
                {
                    if (stream == null)
                    {
                        throw new NullReferenceException();
                    }
                    var content = new byte[stream.Length];
                    stream.Seek(0, SeekOrigin.Begin);
                    await stream.ReadAsync(content, 0, (int)stream.Length, token).ConfigureAwait(false);
                    _abiContract =  Encoding.UTF8.GetString(content);
                }
            }

            return _abiContract;
        }

        public async Task<decimal> GetBalanceAsync(string senderAddress, CancellationToken token)
        {
            var contract = await GetContractAsync(GenerateWeb3Instance(), token).ConfigureAwait(false);
            var function = contract.GetFunction("balanceOf");
            var parameters = (new object[] { senderAddress });
            var result = await function.CallAsync<BigInteger>(parameters).ConfigureAwait(false);
            var normalAmount = result / new BigInteger(FromWei);
            return (decimal) normalAmount;
        }

        public async Task<string> TransferMoneyAsync(string senderPk, string toAddress, float amount, CancellationToken token)
        {
            var contract = await GetContractAsync(GenerateWeb3Instance(senderPk), token).ConfigureAwait(false);
            var operationToExe = contract.GetFunction("transfer");
            var maxGas = new HexBigInteger(70000);
            var amountTransformed = new BigInteger(amount * FromWei);
            var parameters = (new object[] { toAddress, amountTransformed });
            var receiptFirstAmountSend = await operationToExe.SendTransactionAndWaitForReceiptAsync(GetPublicAddress(senderPk), maxGas, null, null, parameters).ConfigureAwait(false);
            return receiptFirstAmountSend.BlockHash;
        }

        public string GetPublicAddress(string privateKey)
        {
            var account = new Account(privateKey);
            return account.Address;
        }

        public Account CreateAccount()
        {
            var ecKey = Nethereum.Signer.EthECKey.GenerateKey();
            var privateKey = ecKey.GetPrivateKeyAsBytes().ToHex();
            return new Account(privateKey);
        }

        public async Task<bool> SetInitialBalanceAsync(string address, CancellationToken token)
        {
            await TransferMoneyAsync("10f158cd550649e9f99e48a9c7e2547b65f101a2f928c3e0172e425067e51bb4", address, 10, token).ConfigureAwait(false);
            return true;
        }

        //public async Task<string> BuyTokens(string senderPK, int amount, CancellationToken token)
        //{

        //    var contract = await GetContractAsync(token, GenerateWeb3Instance(senderPK)).ConfigureAwait(false);
        //    var operationToExe = contract.GetFunction("");
        //    var maxGas = new HexBigInteger(70000);
        //    BigInteger Amount = new BigInteger(amount * Math.Pow(10, 18));
        //    var receiptFirstAmountSend = await operationToExe.SendTransactionAndWaitForReceiptAsync(GetPublicAddress(senderPK), maxGas, new HexBigInteger(100), null);
        //    return receiptFirstAmountSend.ToString();
        //}
    }
}
