using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Infrastructure.BlockChain;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Cloudents.Infrastructure.Blockchain
{
    public static class FunctionExtensions
    {

        public static Task<TransactionReceipt> SendTransactionAndWaitForReceiptAsync(this Function function,
            string privateKey, CancellationToken receiptRequestCancellationToken, params object[] functionInput)
        {

            var publicAddress =
                BlockChainProvider.GetPublicAddress(
                    privateKey);
            return SendTransactionAndWaitForReceiptAsync(function, publicAddress, BlockChainProvider.MaxGas, receiptRequestCancellationToken,
                functionInput);
        }

        public static async Task<TransactionReceipt> SendTransactionAndWaitForReceiptAsync(this Function function, string privateKey, double maxGas,
            CancellationToken receiptRequestCancellationToken, params object[] functionInput)
        {
            var gas = new HexBigInteger((BigInteger)maxGas);
            var publicAddress =
                BlockChainProvider.GetPublicAddress(
                    privateKey);

            using (var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(receiptRequestCancellationToken))
            {
                //var maxGas = new HexBigInteger((System.Numerics.BigInteger)4.7e6);
                return await function.SendTransactionAndWaitForReceiptAsync(publicAddress, gas, null,
                      tokenSource, functionInput).ConfigureAwait(false);
            }

            //return SendTransactionAndWaitForReceiptAsync(function, publicAddress,  receiptRequestCancellationToken,
            //    functionInput);
        }


        //public static Task<TransactionReceipt> SendTransactionAndWaitForReceiptAsync(this Function function, string address, 
        //    CancellationToken receiptRequestCancellationToken, params object[] functionInput)
        //{
        //    using (var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(receiptRequestCancellationToken))
        //    {
        //        var maxGas = new HexBigInteger((System.Numerics.BigInteger)4.7e6);
        //        return function.SendTransactionAndWaitForReceiptAsync(address, maxGas, null,
        //            tokenSource, functionInput);
        //    }
        //}


        //public static Task<TransactionReceipt> XXXX(this Function function, string address, HexBigInteger gas,
        //    CancellationToken receiptRequestCancellationToken, params object[] functionInput)
        //{
        //    using (var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(receiptRequestCancellationToken))
        //    {
        //        var maxGas = new HexBigInteger(gas);
        //        return function.SendTransactionAndWaitForReceiptAsync(address, maxGas, null,
        //            tokenSource, functionInput);
        //    }
        //}
    }
}