using Cloudents.Core.Interfaces;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexTypes;
using Nethereum.Contracts;

namespace Cloudents.Infrastructure.Blockchain
{
    public class BlockchainProvider : IBlockchainProvider
    {
        private readonly IConfigurationKeys _configurationKeys;

        public BlockchainProvider(IConfigurationKeys configurationKeys)
        {
            _configurationKeys = configurationKeys;
        }

        //public async Task<BigInteger> GetBalanceAsync(string accountAddress)
        //{
        //    var chain = new Web3(_configurationKeys.BlockChainNetwork);
        //    var balance = await chain.Eth.GetBalance.SendRequestAsync(accountAddress);
        //    //Console.WriteLine("Account balance: {0}", Web3.Convert.FromWei(balance.Value));
        //    return balance;
        //}

        //public async Task<int> SendTxAsync(string senderAddress, string senderPK, string recipientAddress, string azureUrl)
        //{
        //    var account = new Account(senderPK);
        //    var web3 = new Web3(account, azureUrl);
        //    var wei = Web3.Convert.ToWei(0.05);
        //    var transaction = await web3.TransactionManager.SendTransactionAsync(account.Address, recipientAddress, new HexBigInteger(wei));
        //    return transaction.GetHashCode();
        //}

        private async Task<Contract> GetContractAsync(string senderPK)
        {
            const string abi = @"[{'anonymous': false,'inputs': [{'indexed': true,'name': 'from','type': 'address'},{'indexed': false,'name': 'value','type': 'uint256'}],'name': 'Burn','type': 'event'},{'constant': false,'inputs': [
            {'name': '_spender','type': 'address'},{'name': '_value','type': 'uint256'}],'name': 'approve',	'outputs': [{'name': 'success','type': 'bool'}],'payable': false,'stateMutability': 'nonpayable',
            'type': 'function'},{'constant': false,'inputs': [{'name': '_spender','type': 'address'},{'name': '_value','type': 'uint256'},{'name': '_extraData','type': 'bytes'}],'name': 'approveAndCall','outputs': [{'name': 'success',
            'type': 'bool'}],'payable': false,'stateMutability': 'nonpayable','type': 'function'},{'constant': false,'inputs': [{'name': '_value','type': 'uint256'}],'name': 'burn','outputs': [{'name': 'success','type': 'bool'}],
            'payable': false,'stateMutability': 'nonpayable','type': 'function'},{'constant': false,'inputs': [{'name': '_from','type': 'address'},{'name': '_value','type': 'uint256'}],'name': 'burnFrom','outputs': [
            {'name': 'success','type': 'bool'}],'payable': false,'stateMutability': 'nonpayable','type': 'function'},{'constant': false,'inputs': [],'name': 'buy','outputs': [],'payable': true,'stateMutability': 'payable',
            'type': 'function'},{'constant': false,	'inputs': [{'name': 'target','type': 'address'},{'name': 'freeze','type': 'bool'}],'name': 'freezeAccount','outputs': [],'payable': false,'stateMutability': 'nonpayable','type': 'function'
            },{'constant': false,'inputs': [{'name': 'target','type': 'address'},{'name': 'mintedAmount','type': 'uint256'}],'name': 'mintToken','outputs': [],'payable': false,'stateMutability': 'nonpayable','type': 'function'},
            {'constant': false,'inputs': [{'name': 'amount','type': 'uint256'}],'name': 'sell','outputs': [],'payable': false,'stateMutability': 'nonpayable','type': 'function'},{'anonymous': false,'inputs': [{
            'indexed': true,'name': 'from','type': 'address'},{'indexed': true,'name': 'to','type': 'address'},{'indexed': false,'name': 'value','type': 'uint256'}],'name': 'Transfer','type': 'event'},{'anonymous': false,
            'inputs': [{'indexed': false,'name': 'target','type': 'address'},{'indexed': false,'name': 'frozen','type': 'bool'}],'name': 'FrozenFunds','type': 'event'},{'constant': false,'inputs': [{'name': 'newSellPrice',
            'type': 'uint256'},{'name': 'newBuyPrice','type': 'uint256'}],'name': 'setPrices','outputs': [],'payable': false,'stateMutability': 'nonpayable','type': 'function'},{'constant': false,'inputs': [{'name': '_to',
            'type': 'address'},{'name': '_value','type': 'uint256'}],'name': 'transfer','outputs': [],'payable': false,'stateMutability': 'nonpayable',	'type': 'function'},{'constant': false,'inputs': [{	'name': '_from',
            'type': 'address'},{'name': '_to','type': 'address'},{'name': '_value',	'type': 'uint256'}],'name': 'transferFrom','outputs': [{'name': 'success','type': 'bool'}],'payable': false,'stateMutability': 'nonpayable',
            'type': 'function'},{'constant': false,'inputs': [{'name': 'newOwner','type': 'address'}],'name': 'transferOwnership','outputs': [],'payable': false,'stateMutability': 'nonpayable','type': 'function'},{
            'inputs': [{'name': 'initialSupply','type': 'uint256'},{'name': 'tokenName','type': 'string'},{'name': 'tokenSymbol','type': 'string'}],'payable': false,'stateMutability': 'nonpayable','type': 'constructor'
            },{'constant': true,'inputs': [{'name': '','type': 'address'},{'name': '','type': 'address'}],'name': 'allowance','outputs': [{'name': '','type': 'uint256'}],'payable': false,'stateMutability': 'view',
            'type': 'function'},{'constant': true,'inputs': [{'name': '','type': 'address'}],'name': 'balanceOf','outputs': [{'name': '','type': 'uint256'}],'payable': false,'stateMutability': 'view','type': 'function'
            },{'constant': true,'inputs': [],'name': 'buyPrice','outputs': [{'name': '','type': 'uint256'}],'payable': false,'stateMutability': 'view','type': 'function'},{'constant': true,'inputs': [],'name': 'decimals',
            'outputs': [{'name': '','type': 'uint8'}],'payable': false,'stateMutability': 'view','type': 'function'},{'constant': true,'inputs': [{	'name': '','type': 'address'}],'name': 'frozenAccount','outputs': [{
            'name': '','type': 'bool'}],'payable': false,'stateMutability': 'view','type': 'function'},{'constant': true,'inputs': [],'name': 'name','outputs': [{'name': '','type': 'string'}],'payable': false,'stateMutability': 'view',
            'type': 'function'},{'constant': true,'inputs': [],'name': 'owner','outputs': [{'name': '',	'type': 'address'}],'payable': false,'stateMutability': 'view','type': 'function'},{'constant': true,'inputs': [],'name': 'sellPrice',
            'outputs': [{'name': '','type': 'uint256'}],'payable': false,'stateMutability': 'view','type': 'function'},{'constant': true,'inputs': [],'name': 'symbol','outputs': [{'name': '','type': 'string'}],'payable': false,'stateMutability'
            : 'view','type': 'function'},{'constant': true,'inputs': [],'name': 'totalSupply','outputs': [{'name': '','type': 'uint256'}],'payable': false,'stateMutability': 'view','type': 'function'}]";
            //ICO abi

            const string transactionHash = "0xa09db301ad49fb1e240f7fe6c4a70edadd9506d93278fb412b571cf8b2786aa4"; //ICO Contract Hash

            Account account = new Account(senderPK);
            var web3 = new Web3(account, _configurationKeys.BlockChainNetwork);
            var DeploymentReceipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            while (DeploymentReceipt == null)
            {
                await Task.Delay(TimeSpan.FromSeconds(0.5));
                DeploymentReceipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            }
            var contractAddress = DeploymentReceipt.ContractAddress;
            return web3.Eth.GetContract(abi, contractAddress);
           // var operationToExe = contract.GetFunction(operation);
           // operationToExe.
           // return Task.FromResult(string.Empty);
        }

        public async Task<BigInteger> GetTokenBalanceAsync(string senderPK)
        {
            var contract = await GetContractAsync(senderPK);
            var function = contract.GetFunction("balanceOf");
            var parameters = (new object[] {(object)GetPublicAddress(senderPK)});
            BigInteger result = await function.CallAsync<BigInteger>(parameters);
            return result;
        }

        public async Task<string> TransferMoneyAsync(string senderPK, string toAddress, float amount)
        {
            var contract = await GetContractAsync(senderPK);
            var operationToExe = contract.GetFunction("transfer");
            var maxGas = new HexBigInteger(70000);
            BigInteger Amount = new BigInteger(amount * Math.Pow(10,18));
            var parameters = (new object[] { (object)toAddress, (object) Amount});
            var receiptFirstAmountSend = await operationToExe.SendTransactionAndWaitForReceiptAsync(GetPublicAddress(senderPK), maxGas, null, null, parameters);
            return receiptFirstAmountSend.BlockHash;
        }

        public string GetPublicAddress(string privateKey)
        {
            var account = new Nethereum.Web3.Accounts.Account(privateKey);
            return account.Address;
        }

        public Account CreateAccount ()
        {
            var ecKey = Nethereum.Signer.EthECKey.GenerateKey();
            var privateKey = ecKey.GetPrivateKeyAsBytes().ToHex();
            var account = new Account(privateKey);
            return account;
        }

        public async Task<bool> SetInitialBalance (string address)
        {
            await TransferMoneyAsync("10f158cd550649e9f99e48a9c7e2547b65f101a2f928c3e0172e425067e51bb4", address, 10);
            return true;
        }
    }
}
