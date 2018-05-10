using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.HdWallet;
using System.Threading;
using Nethereum.Hex.HexTypes;


namespace Cloudents.Infrastructure.Blockchain
{
   public class BlockchainProvider : IBlockchainProvider
    {
       
        public async Task<long> GetBalanceAsync(string networkUrl, string accountAddress)
        {
            var chain = new Web3(networkUrl);
            var balance = await chain.Eth.GetBalance.SendRequestAsync(accountAddress);
            Console.WriteLine("Account balance: {0}", Web3.Convert.FromWei(balance.Value));
            return Convert.ToInt64(balance);
        }

        public string CreateAccount()
        {
            var ecKey = Nethereum.Signer.EthECKey.GenerateKey();
            var privateKey = ecKey.GetPrivateKeyAsBytes().ToHex();
            var account = new Nethereum.Web3.Accounts.Account(privateKey);
            return account.PrivateKey;
        }


        public async Task<int> SendTxAsync(string senderAddress, string senderPK, string recipientAddress, string azureUrl)
        {
            var account = new Account(senderPK);
            var web3 = new Web3(account, azureUrl);
            var wei = Web3.Convert.ToWei(0.05);
            var transaction = await web3.TransactionManager.SendTransactionAsync(account.Address, recipientAddress, new HexBigInteger(wei));
            return transaction.GetHashCode();
        }

        public async Task<T> MessageContractAsync<T>(string contractHash, string azureUrl, string abi, string functionName)
        {
            var web3 = new Web3(azureUrl);
            var receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(contractHash);
            while (receipt == null)
            {
                Thread.Sleep(5000);
                receipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(contractHash);
            }
            var contractAddress = receipt.ContractAddress;
            var contract = web3.Eth.GetContract(abi, contractAddress);
            var function = contract.GetFunction(functionName);
            var result = await function.CallAsync<T>();
            return result;
        }


        public async Task<string> TxContractAsync(string operation, string senderAddress, string senderPK, string contractHash, string abi, string azureUrl, string[] parameters)
        {
            
            Account account = new Account(senderPK);
            var web3 = new Web3(account, azureUrl);
            var DeploymentReceipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(contractHash);
            while (DeploymentReceipt == null)
            {
                Thread.Sleep(5000);
                DeploymentReceipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(contractHash);
            }
            var contractAddress = DeploymentReceipt.ContractAddress;
            var contract = web3.Eth.GetContract(abi, contractAddress);
            var operationToExe = contract.GetFunction(operation);
            
            var result = await operationToExe.SendTransactionAsync(senderAddress, new HexBigInteger(70000), new HexBigInteger(1), parameters); //Working
            return result;
        }

        public string GetPublicAddress(string privateKey)
        {
            var account = new Nethereum.Web3.Accounts.Account(privateKey);
            return account.Address;
        }

    }
}
