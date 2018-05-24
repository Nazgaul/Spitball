using Cloudents.Core.Interfaces;
using System;
using System.IO;
using System.Numerics;
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

        public BlockChainProvider(IConfigurationKeys configurationKeys)
        {
            _configurationKeys = configurationKeys;
        }

        private Web3 GenerateWeb3Instance(string senderPk = null) {
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

        private async Task<Contract> GetContractAsync(CancellationToken token, Web3 web3)
        {
            const string abi = @"[
	{
		'anonymous': false,
		'inputs': [
			{
				'indexed': true,
				'name': 'from',
				'type': 'address'
			},
			{
				'indexed': false,
				'name': 'value',
				'type': 'uint256'
			}
		],
		'name': 'Burn',
		'type': 'event'
	},
	{
		'constant': false,
		'inputs': [
			{
				'name': '_spender',
				'type': 'address'
			},
			{
				'name': '_value',
				'type': 'uint256'
			}
		],
		'name': 'approve',
		'outputs': [
			{
				'name': 'success',
				'type': 'bool'
			}
		],
		'payable': false,
		'stateMutability': 'nonpayable',
		'type': 'function'
	},
	{
		'constant': false,
		'inputs': [
			{
				'name': '_value',
				'type': 'uint256'
			}
		],
		'name': 'burn',
		'outputs': [
			{
				'name': 'success',
				'type': 'bool'
			}
		],
		'payable': false,
		'stateMutability': 'nonpayable',
		'type': 'function'
	},
	{
		'constant': false,
		'inputs': [
			{
				'name': '_from',
				'type': 'address'
			},
			{
				'name': '_value',
				'type': 'uint256'
			}
		],
		'name': 'burnFrom',
		'outputs': [
			{
				'name': 'success',
				'type': 'bool'
			}
		],
		'payable': false,
		'stateMutability': 'nonpayable',
		'type': 'function'
	},
	{
		'constant': false,
		'inputs': [],
		'name': 'buy',
		'outputs': [],
		'payable': true,
		'stateMutability': 'payable',
		'type': 'function'
	},
	{
		'constant': false,
		'inputs': [
			{
				'name': 'target',
				'type': 'address'
			},
			{
				'name': 'freeze',
				'type': 'bool'
			}
		],
		'name': 'freezeAccount',
		'outputs': [],
		'payable': false,
		'stateMutability': 'nonpayable',
		'type': 'function'
	},
	{
		'constant': false,
		'inputs': [
			{
				'name': 'target',
				'type': 'address'
			},
			{
				'name': 'mintedAmount',
				'type': 'uint256'
			}
		],
		'name': 'mintToken',
		'outputs': [],
		'payable': false,
		'stateMutability': 'nonpayable',
		'type': 'function'
	},
	{
		'constant': false,
		'inputs': [
			{
				'name': 'amount',
				'type': 'uint256'
			}
		],
		'name': 'sell',
		'outputs': [],
		'payable': false,
		'stateMutability': 'nonpayable',
		'type': 'function'
	},
	{
		'constant': false,
		'inputs': [
			{
				'name': 'newSellPrice',
				'type': 'uint256'
			},
			{
				'name': 'newBuyPrice',
				'type': 'uint256'
			}
		],
		'name': 'setPrices',
		'outputs': [],
		'payable': false,
		'stateMutability': 'nonpayable',
		'type': 'function'
	},
	{
		'anonymous': false,
		'inputs': [
			{
				'indexed': true,
				'name': 'from',
				'type': 'address'
			},
			{
				'indexed': true,
				'name': 'to',
				'type': 'address'
			},
			{
				'indexed': false,
				'name': 'value',
				'type': 'uint256'
			}
		],
		'name': 'Transfer',
		'type': 'event'
	},
	{
		'anonymous': false,
		'inputs': [
			{
				'indexed': false,
				'name': 'target',
				'type': 'address'
			},
			{
				'indexed': false,
				'name': 'frozen',
				'type': 'bool'
			}
		],
		'name': 'FrozenFunds',
		'type': 'event'
	},
	{
		'constant': false,
		'inputs': [
			{
				'name': '_to',
				'type': 'address'
			},
			{
				'name': '_value',
				'type': 'uint256'
			}
		],
		'name': 'transfer',
		'outputs': [],
		'payable': false,
		'stateMutability': 'nonpayable',
		'type': 'function'
	},
	{
		'constant': false,
		'inputs': [
			{
				'name': '_from',
				'type': 'address'
			},
			{
				'name': '_to',
				'type': 'address'
			},
			{
				'name': '_value',
				'type': 'uint256'
			}
		],
		'name': 'transferFrom',
		'outputs': [
			{
				'name': 'success',
				'type': 'bool'
			}
		],
		'payable': false,
		'stateMutability': 'nonpayable',
		'type': 'function'
	},
	{
		'constant': false,
		'inputs': [
			{
				'name': 'newOwner',
				'type': 'address'
			}
		],
		'name': 'transferOwnership',
		'outputs': [],
		'payable': false,
		'stateMutability': 'nonpayable',
		'type': 'function'
	},
	{
		'inputs': [
			{
				'name': 'initialSupply',
				'type': 'uint256'
			},
			{
				'name': 'tokenName',
				'type': 'string'
			},
			{
				'name': 'tokenSymbol',
				'type': 'string'
			}
		],
		'payable': false,
		'stateMutability': 'nonpayable',
		'type': 'constructor'
	},
	{
		'constant': true,
		'inputs': [
			{
				'name': '',
				'type': 'address'
			},
			{
				'name': '',
				'type': 'address'
			}
		],
		'name': 'allowance',
		'outputs': [
			{
				'name': '',
				'type': 'uint256'
			}
		],
		'payable': false,
		'stateMutability': 'view',
		'type': 'function'
	},
	{
		'constant': true,
		'inputs': [
			{
				'name': '',
				'type': 'address'
			}
		],
		'name': 'balanceOf',
		'outputs': [
			{
				'name': '',
				'type': 'uint256'
			}
		],
		'payable': false,
		'stateMutability': 'view',
		'type': 'function'
	},
	{
		'constant': true,
		'inputs': [],
		'name': 'buyPrice',
		'outputs': [
			{
				'name': '',
				'type': 'uint256'
			}
		],
		'payable': false,
		'stateMutability': 'view',
		'type': 'function'
	},
	{
		'constant': true,
		'inputs': [],
		'name': 'decimals',
		'outputs': [
			{
				'name': '',
				'type': 'uint8'
			}
		],
		'payable': false,
		'stateMutability': 'view',
		'type': 'function'
	},
	{
		'constant': true,
		'inputs': [
			{
				'name': '',
				'type': 'address'
			}
		],
		'name': 'frozenAccount',
		'outputs': [
			{
				'name': '',
				'type': 'bool'
			}
		],
		'payable': false,
		'stateMutability': 'view',
		'type': 'function'
	},
	{
		'constant': true,
		'inputs': [],
		'name': 'name',
		'outputs': [
			{
				'name': '',
				'type': 'string'
			}
		],
		'payable': false,
		'stateMutability': 'view',
		'type': 'function'
	},
	{
		'constant': true,
		'inputs': [],
		'name': 'owner',
		'outputs': [
			{
				'name': '',
				'type': 'address'
			}
		],
		'payable': false,
		'stateMutability': 'view',
		'type': 'function'
	},
	{
		'constant': true,
		'inputs': [],
		'name': 'sellPrice',
		'outputs': [
			{
				'name': '',
				'type': 'uint256'
			}
		],
		'payable': false,
		'stateMutability': 'view',
		'type': 'function'
	},
	{
		'constant': true,
		'inputs': [],
		'name': 'symbol',
		'outputs': [
			{
				'name': '',
				'type': 'string'
			}
		],
		'payable': false,
		'stateMutability': 'view',
		'type': 'function'
	},
	{
		'constant': true,
		'inputs': [],
		'name': 'totalSupply',
		'outputs': [
			{
				'name': '',
				'type': 'uint256'
			}
		],
		'payable': false,
		'stateMutability': 'view',
		'type': 'function'
	}
]";
            //ICO abi
          
            const string transactionHash = "0xbf640f0b58fcad57ccd2eea9946df1f113e08bdb5aa0a4a932500546811a7beb"; //ICO Contract Hash
          


            var deploymentReceipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash).ConfigureAwait(false);
            while (deploymentReceipt == null)
            {
                await Task.Delay(TimeSpan.FromSeconds(0.5), token).ConfigureAwait(false);
                deploymentReceipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash).ConfigureAwait(false);
            }
            var contractAddress = deploymentReceipt.ContractAddress;
            return web3.Eth.GetContract(ReadApi(), contractAddress);
        }


        private static string _abiContract;

        private static string ReadApi()
        {
            if (_abiContract == null)
            {
                _abiContract = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "abi.json"));
            }

            return _abiContract;
        }

     
          
        

        public async Task<BigInteger> GetTokenBalanceAsync(string senderAddress, CancellationToken token)
        {
            var contract = await GetContractAsync(token, GenerateWeb3Instance());
            var function = contract.GetFunction("balanceOf");
            var parameters = (new object[] { senderAddress });
            return await function.CallAsync<BigInteger>(parameters);
        }

        public async Task<string> TransferMoneyAsync(string senderPk, string toAddress, float amount, CancellationToken token)
        {
            var contract = await GetContractAsync(token, GenerateWeb3Instance(senderPk)).ConfigureAwait(false);
            var operationToExe = contract.GetFunction("transfer");
            var maxGas = new HexBigInteger(70000);
            var amountTransformed = new BigInteger(amount * Math.Pow(10, 18));
            var parameters = (new object[] { toAddress, amountTransformed });
            var receiptFirstAmountSend = await operationToExe.SendTransactionAndWaitForReceiptAsync(GetPublicAddress(senderPk), maxGas, null, null, parameters);
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
