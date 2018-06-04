using Cloudents.Core.Interfaces;
using System;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Contracts;

namespace Cloudents.Infrastructure.BlockChain
{
    public abstract class BlockChainProvider
    {
        private readonly IConfigurationKeys _configurationKeys;

        protected const double FromWei = 1e18;
        public const double MaxGas = 4.7e6;
        protected const string SpitballPrivateKey = "10f158cd550649e9f99e48a9c7e2547b65f101a2f928c3e0172e425067e51bb4";


        //private const string ICOtransactionHash = "0x430fdc71d7b86f432ae0d22d0cc11ce7909f0434942f5943f2288f3140dac07d";

        //"0xfea5f8e467e7423ec7304bdc220dfa415147848546a3f52a310c91df3bd5f6fe";
        //"0xbf640f0b58fcad57ccd2eea9946df1f113e08bdb5aa0a4a932500546811a7beb"; //ICO Contract Hash

        protected BlockChainProvider(IConfigurationKeys configurationKeys)
        {
            _configurationKeys = configurationKeys;
        }

        protected Web3 GenerateWeb3Instance(string senderPk = null)
        {
            if (senderPk != null)
            {
                var account = new Account(senderPk);
                return new Web3(account, _configurationKeys.BlockChainNetwork);
            }
            return new Web3(_configurationKeys.BlockChainNetwork);
        }

        protected abstract string Abi { get; }
        protected abstract string TransactionHash { get; }
        protected abstract string ContractAddress { get; }

        


        protected async Task<Contract> GetContractAsync(Web3 web3, CancellationToken token)
        {
            //var deploymentReceipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(TransactionHash).ConfigureAwait(false);
            //while (deploymentReceipt == null)
            //{
            //    await Task.Delay(TimeSpan.FromSeconds(0.5), token).ConfigureAwait(false);
            //    deploymentReceipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(TransactionHash).ConfigureAwait(false);
            //}
            //var contractAddress = deploymentReceipt.ContractAddress;
            var abi = await ReadAbiAsync(token).ConfigureAwait(false);
            return web3.Eth.GetContract(abi, ContractAddress);
        }

        private static string _abiContract;

        private async Task<string> ReadAbiAsync(CancellationToken token)
        {
            if (_abiContract != null) return _abiContract;
            using (var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("Cloudents.Infrastructure.Blockchain." + Abi + ".json"))
            {
                if (stream == null)
                {
                    throw new NullReferenceException();
                }
                using (var reader = new StreamReader(stream))
                {
                    _abiContract = await reader.ReadToEndAsync().ConfigureAwait(false);
                    token.ThrowIfCancellationRequested();
                }
            }

            return _abiContract;
        }

        public static string GetPublicAddress(string privateKey)
        {
            var address = Web3.GetAddressFromPrivateKey(privateKey);
            return address;
        }

        public static (string privateKey, string publicAddress) CreateAccount()
        {
            var ecKey = Nethereum.Signer.EthECKey.GenerateKey();
            var privateKey = ecKey.GetPrivateKeyAsBytes();
            string address = Web3.GetAddressFromPrivateKey(privateKey.ToHex());
            return (privateKey.ToHex(), address);
        }
    }
}
