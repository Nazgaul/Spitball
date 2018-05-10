using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Numerics;
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
       private readonly IConfigurationKeys _configurationKeys;

       public BlockchainProvider(IConfigurationKeys configurationKeys)
       {
           _configurationKeys = configurationKeys;
       }


       public async Task<BigInteger> GetBalanceAsync(string accountAddress)
        {
            var chain = new Web3(_configurationKeys.BlockChainNetwork);
            var balance = await chain.Eth.GetBalance.SendRequestAsync(accountAddress);
            //Console.WriteLine("Account balance: {0}", Web3.Convert.FromWei(balance.Value));
            return balance;
        }

        public string CreateAccount()
        {
            var ecKey = Nethereum.Signer.EthECKey.GenerateKey();
            var privateKey = ecKey.GetPrivateKeyAsBytes().ToHex();
            var account = new Account(privateKey);
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


        //public async Task<string> TxContractAsync(string operation, string senderAddress, string senderPK, string contractHash, string abi, string azureUrl)
        //{
        //    //byte[] bytes = senderSeed.HexToByteArray();  // Hadar's privete key
        //    //var wallet = new Wallet(bytes).GetAccount(senderAddress);
        //    Account account = new Account(senderPK);
        //    var web3 = new Web3(account, azureUrl);
        //    var DeploymentReceipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(contractHash);
        //    while (DeploymentReceipt == null)
        //    {
        //        Thread.Sleep(5000);
        //        DeploymentReceipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(contractHash);
        //    }
        //    var contractAddress = DeploymentReceipt.ContractAddress;
        //    var contract = web3.Eth.GetContract(abi, contractAddress);
        //    var operationToExe = contract.GetFunction(operation);
        //    //var result = await operationToExe.SendTransactionAndWaitForReceiptAsync(address, new HexBigInteger(70000), null, null); Works too
        //    var result = await operationToExe.SendTransactionAsync(senderAddress, new HexBigInteger(70000), null); //Working
        //    return result;
        //}

        public string GetPublicAddress(string privateKey)
        {
            var account = new Nethereum.Web3.Accounts.Account(privateKey);
            return account.Address;
        }

    }
}
