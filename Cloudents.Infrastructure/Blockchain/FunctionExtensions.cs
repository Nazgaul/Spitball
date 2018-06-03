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
            CancellationToken receiptRequestCancellationToken, params object[] functionInput)
        {
            //var maxGas = new HexBigInteger(4.7e6);
            var publicAddress =
                BlockChainProvider.GetPublicAddress(
                    "10f158cd550649e9f99e48a9c7e2547b65f101a2f928c3e0172e425067e51bb4");
            return SendTransactionAndWaitForReceiptAsync(function, publicAddress,  receiptRequestCancellationToken,
                functionInput);
        }


        public static Task<TransactionReceipt> SendTransactionAndWaitForReceiptAsync(this Function function, string address, 
            CancellationToken receiptRequestCancellationToken, params object[] functionInput)
        {
            using (var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(receiptRequestCancellationToken))
            {
                var maxGas = new HexBigInteger((System.Numerics.BigInteger)4.7e6);
                return function.SendTransactionAndWaitForReceiptAsync(address, maxGas, null,
                    tokenSource, functionInput);
            }
        }


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