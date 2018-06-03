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
using Cloudents.Infrastructure.BlockChain;

namespace Cloudents.Infrastructure.Blockchain
{
    class Erc20Service : BlockChainProvider, IBlockChainErc20Service
    {
        protected override string Abi => "TokenAbi";

        protected override string TransactionHash => "0x430fdc71d7b86f432ae0d22d0cc11ce7909f0434942f5943f2288f3140dac07d";
        protected override string ContractAddress => "0x55a885a9a1f7e8e5ca10a79ad7addcc5bc43f623";

        public Erc20Service (IConfigurationKeys configurationKeys) : base(configurationKeys)
        {
        }

        public async Task<decimal> GetBalanceAsync(string senderAddress, CancellationToken token)
        {
            var contract = await GetContractAsync(GenerateWeb3Instance(),  token).ConfigureAwait(false);
            var function = contract.GetFunction("balanceOf");
            var parameters = (new object[] { senderAddress });
            var result = await function.CallAsync<BigInteger>(parameters).ConfigureAwait(false);
            var normalAmount = result / new BigInteger(FromWei);
            return (decimal)normalAmount;
        }

        public async Task<string> TransferMoneyAsync(string senderPk, string toAddress, float amount, CancellationToken token)
        {
          
            var contract = await GetContractAsync(GenerateWeb3Instance(senderPk), token).ConfigureAwait(false);
            var operationToExe = contract.GetFunction("transfer");
            var maxGas = new HexBigInteger(70000);
            var amountTransformed = new BigInteger(amount * FromWei);
            var parameters = new object[] { toAddress, amountTransformed };
            var receiptFirstAmountSend = await operationToExe.SendTransactionAndWaitForReceiptAsync(GetPublicAddress(senderPk), maxGas, null, null, parameters).ConfigureAwait(false);
            return receiptFirstAmountSend.BlockHash;
        }

        public async Task<bool> SetInitialBalanceAsync(string address, CancellationToken token)
        {
            await TransferMoneyAsync("10f158cd550649e9f99e48a9c7e2547b65f101a2f928c3e0172e425067e51bb4", address, 10, token).ConfigureAwait(false);
            return true;
        }

    }
}
