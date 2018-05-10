using System;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.HdWallet;
using System.Threading;
using Nethereum.Hex.HexTypes;

//var unlockResult = await web3.Personal.UnlockAccount.SendRequestAsync(senderAddress, password, new HexBigInteger(120));

namespace ConsoleApp
{
    class EthereumBackEnd
    {
        static void Pr(string[] args)
        {

            try
            {
                string azureUrl = "http://s256cw-dns-reg1.northeurope.cloudapp.azure.com:8545";
               
                //var transactionHash = "0x9aaf84d4eb832fc6f6bb2518f276ddda24f34fbd365e2bab079a600b2a0ae92e"; //HelloWorld Contract Hash
                var transactionHash = "0xfdf23b79553c3202403ca5482b8393a6515f25d60377bf7e80c942b36cc1659e"; //ICO Contract Hash
                //var abi = @"[{'constant': false,'inputs': [],'name': 'add','outputs': [],'payable': false,'stateMutability': 'nonpayable','type': 'function'},{'constant': false,'inputs': [],'name': 'subtract','outputs': [],'payable': false,'stateMutability': 'nonpayable','type': 'function'},{'constant': true,'inputs': [],'name': 'getCounter','outputs': [{'name': '','type': 'uint256'}],'payable': false,'stateMutability': 'view','type': 'function'}]";

                var abi = @"[{'anonymous': false,'inputs': [{'indexed': true,'name': 'from','type': 'address'},{'indexed': false,'name': 'value','type': 'uint256'}],'name': 'Burn','type': 'event'},{'constant': false,'inputs': [
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

                string FromAddress = "0x27e739f9dF8135fD1946b0b5584BcE49E22000af"; // Hadar's Address
                string ToAddress = "0x828AB82CFec33d6Ee2D1d7471b4e52440f6cb446"; // Irena's Addres
                string SenderSeed = "160b2006f43b9aae82f72e0d15e23154f18c86d44a8e6eb51b214b6a2240785ce130bdc969a61b62b8baa18562704ff6f27d7c9ea5e2ac879159f6e912ae5619"; // Hadar's seed
                
                Account ac = CreateAccount();
                Console.WriteLine(ac.NonceService);
                //TxContract("add", FromAddress, SenderSeed, transactionHash, abi, azureUrl);
                ////TalkToContract(transactionHash, azureUrl, abi);
                //SendTx(FromAddress, SenderSeed, ToAddress, azureUrl);
            }
            catch
            {
                Console.Write("error");
            }
            Console.ReadLine();
        }


        static async void GetBalance(string networkUrl, string accountAddress)
        {
            var chain = new Web3(networkUrl);
            var balance = await chain.Eth.GetBalance.SendRequestAsync(accountAddress);
            Console.WriteLine("Account balance: {0}", Web3.Convert.FromWei(balance.Value));
        }



        static async void SendTx(string senderAddress, string senderSeed, string recipientAddress, string azureUrl)
        {
            //var wordlist = "vote shaft stuff buzz clown ahead faint autumn walnut hood person captain"; //Hadar's account wordlist
            //var wallet = new Wallet(wordlist, null).Seed;// .GetAccount(SenderAddress);
            // var address = "0x27e739f9dF8135fD1946b0b5584BcE49E22000af"; //Hadar's account address

            byte[] bytes = senderSeed.HexToByteArray();
            var wallet = new Wallet(bytes).GetAccount(senderAddress);
            
            var web3 = new Web3(wallet, azureUrl);
            var wei = Web3.Convert.ToWei(0.05);
            var transaction = await web3.TransactionManager.SendTransactionAsync(wallet.Address, recipientAddress, new HexBigInteger(wei));
            Console.WriteLine(transaction);

        }

        static Account CreateAccount()
        {
            var ecKey = Nethereum.Signer.EthECKey.GenerateKey();
            var privateKey = ecKey.GetPrivateKeyAsBytes().ToHex();
            var account = new Nethereum.Web3.Accounts.Account(privateKey);
            return account;
        }

        static async void TalkToContract(string contractHash, string azureUrl, string abi)
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
            var getCounter = contract.GetFunction("getCounter");
            var result = await getCounter.CallAsync<int>();
            Console.WriteLine(result);
        }

        static async void TxContract(string operation, string senderAddress, string senderSeed, string contractHash, string abi, string azureUrl)
        {
            byte[] bytes = senderSeed.HexToByteArray();  // Hadar's privete key
            var wallet = new Wallet(bytes).GetAccount(senderAddress);
            var web3 = new Web3(wallet, azureUrl);
            var DeploymentReceipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(contractHash);
            while (DeploymentReceipt == null)
            {
                Thread.Sleep(5000);
                DeploymentReceipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(contractHash);
            }
            var contractAddress = DeploymentReceipt.ContractAddress;
            var contract = web3.Eth.GetContract(abi, contractAddress);
            var operationToExe = contract.GetFunction(operation);
            //var result = await operationToExe.SendTransactionAndWaitForReceiptAsync(address, new HexBigInteger(70000), null, null); Works too
            var result = await operationToExe.SendTransactionAsync(senderAddress, new HexBigInteger(70000), null); //Working
            Console.WriteLine(result);
        }

    }
}
