using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace Cloudents.Infrastructure.Blockchain
{
    public static class FunctionExtensions
    {
        //public static Task<TransactionReceipt> SendTransactionAndWaitForReceiptAsync(this Function function,
        //    string privateKey, CancellationToken receiptRequestCancellationToken, params object[] functionInput)
        //{
        //    var publicAddress =
        //        BlockChainProvider.GetPublicAddress(
        //            privateKey);
        //    return SendTransactionAndWaitForReceiptAsync(function, publicAddress, BlockChainProvider.MaxGas, receiptRequestCancellationToken,
        //        functionInput);
        //}

        public static async Task<TransactionReceipt> SendTransactionAndWaitForReceiptAsync(this Function function, string privateKey, double maxGas,
            CancellationToken receiptRequestCancellationToken, params object[] functionInput)
        {
            var gas = new HexBigInteger((BigInteger)maxGas);
            var publicAddress =
                Web3.GetAddressFromPrivateKey(privateKey);

            using (var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(receiptRequestCancellationToken))
            {
                    return await function.SendTransactionAndWaitForReceiptAsync(publicAddress, gas, null,
                        tokenSource, functionInput).ConfigureAwait(false);
            }
        }
    }
}