using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Nethereum.Contracts;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace Cloudents.Infrastructure.Blockchain
{
    public abstract class BlockChainProvider
    {
        private readonly IConfigurationKeys _configurationKeys;

        protected const double FromWei = 1e18;
        protected const double MaxGas = 4.1e6;
        protected const double GasPrice = 30e9;
        protected const string SpitballPrivateKey = "428ac528cbc75b2832f4a46592143f46d3cb887c5822bed23c8bf39d027615a8";

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
        protected abstract string ContractAddress { get; }

        protected async Task<Contract> GetContractAsync(Web3 web3, CancellationToken token)
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

       

        protected async Task<Function> GetFunctionAsync(string name, CancellationToken token)
        {
            var contract = await GetContractAsync(GenerateWeb3Instance(SpitballPrivateKey), token).ConfigureAwait(false);
            return contract.GetFunction(name);
        }

        protected async Task<Function> GetFunctionAsync(string name, string privateKey, CancellationToken token)
        {
            var contract = await GetContractAsync(GenerateWeb3Instance(privateKey), token).ConfigureAwait(false);
            return contract.GetFunction(name);
        }
    }
}
