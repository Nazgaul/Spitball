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
using Nethereum.KeyStore;

namespace Cloudents.Infrastructure.BlockChain
{
    public abstract class BlockChainProvider 
    {
        protected readonly IConfigurationKeys _configurationKeys;

        protected const double FromWei = 1e18;
        //private const string ICOtransactionHash = "0x430fdc71d7b86f432ae0d22d0cc11ce7909f0434942f5943f2288f3140dac07d";
       
            //"0xfea5f8e467e7423ec7304bdc220dfa415147848546a3f52a310c91df3bd5f6fe";
            //"0xbf640f0b58fcad57ccd2eea9946df1f113e08bdb5aa0a4a932500546811a7beb"; //ICO Contract Hash

        public BlockChainProvider(IConfigurationKeys configurationKeys)
        {
            _configurationKeys = configurationKeys;
        }

        protected Web3 GenerateWeb3Instance(string senderPk = null)
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

        protected abstract string Abi { get; }
        protected abstract string TransactionHash { get; }


        protected async Task<Contract> GetContractAsync(Web3 web3, CancellationToken token)
        {
            var deploymentReceipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(TransactionHash).ConfigureAwait(false);
            while (deploymentReceipt == null)
            {
                await Task.Delay(TimeSpan.FromSeconds(0.5), token).ConfigureAwait(false);
                deploymentReceipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(TransactionHash).ConfigureAwait(false);
            }
            var contractAddress = deploymentReceipt.ContractAddress;
            var abi = await ReadAbiAsync( token).ConfigureAwait(false);
            return web3.Eth.GetContract(abi, contractAddress);
        }

        private static string _abiContract;

        protected async Task<string> ReadAbiAsync(CancellationToken token)
        {
            if (_abiContract == null)
            {
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
                    }
                }
            }

            return _abiContract;
        }

        public string GetPublicAddress(string privateKey)
        {
            var account = new Account(privateKey);
            return account.Address;
        }

        public (string privateKey, string publicAddress) CreateAccount()
        {
            var ecKey = Nethereum.Signer.EthECKey.GenerateKey();
            var privateKey = ecKey.GetPrivateKeyAsBytes();
            var account = new Account(privateKey.ToHex());
            
            return (privateKey.ToHex(), account.Address);
        }
    }
}
