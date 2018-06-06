using Cloudents.Core.Interfaces;
using System;
using System.Collections.Concurrent;
using System.IO;
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

        protected BlockChainProvider(IConfigurationKeys configurationKeys)
        {
            _configurationKeys = configurationKeys;
        }

        private Web3 GenerateWeb3Instance(string senderPk = null)
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

        private async Task<Contract> GetContractAsync(Web3 web3, CancellationToken token)
        {
            var abi = await ReadAbiAsync(token).ConfigureAwait(false);
            return web3.Eth.GetContract(abi, ContractAddress);
        }

        private static readonly ConcurrentDictionary<string, Task<string>> Instances = new ConcurrentDictionary<string, Task<string>>();

        private Task<string> ReadAbiAsync(CancellationToken token)

        {
            return Instances.GetOrAdd(ContractAddress, async _ =>
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
                        token.ThrowIfCancellationRequested();
                        return await reader.ReadToEndAsync().ConfigureAwait(false);
                    }
                }
            });
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
            var address = Web3.GetAddressFromPrivateKey(privateKey.ToHex());
            return (privateKey.ToHex(), address);
        }

        protected async Task<Function> GetFunctionAsync(string name, CancellationToken token)
        {
            var contract = await GetContractAsync(GenerateWeb3Instance(SpitballPrivateKey), token).ConfigureAwait(false);
            return contract.GetFunction(name);
        }
    }
}
