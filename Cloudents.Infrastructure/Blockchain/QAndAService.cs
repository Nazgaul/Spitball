using Cloudents.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.Web3;
using Nethereum.Hex.HexTypes;
using Cloudents.Infrastructure.BlockChain;
using System.Collections.Generic;
using JetBrains.Annotations;
using Nethereum.RPC.Eth.DTOs;

namespace Cloudents.Infrastructure.Blockchain
{
    [UsedImplicitly]
    public class QAndAService : BlockChainProvider, IBlockChainQAndAContract
    {
        protected override string Abi => "QAndA";

        protected override string TransactionHash => "0xa976e2a5828a8a53d0f1f76d931613ee40cc0281e977a658a442873a379e1f66";
            //"0x20327f5f3836cfdcbc5b38d49eac517cbf532134973c15a653cac2eb68b65dfd";

        public QAndAService (IConfigurationKeys configurationKeys) : base(configurationKeys)
        {
        }

        public async Task SubmitQuestionAsync(long questionId, decimal price, string userAddress,
            CancellationToken token)
        {
            var contract = await GetContractAsync(GenerateWeb3Instance("10f158cd550649e9f99e48a9c7e2547b65f101a2f928c3e0172e425067e51bb4"), token).ConfigureAwait(false);
            var operationToExe = contract.GetFunction("submitNewQuestion");
            //operationToExe.SendTransactionAndWaitForReceiptAsync(token,)
            var maxGas = new HexBigInteger(3000000);
            var parameters = new object[] { questionId, price, userAddress };
            await operationToExe.SendTransactionAndWaitForReceiptAsync(GetPublicAddress("10f158cd550649e9f99e48a9c7e2547b65f101a2f928c3e0172e425067e51bb4"), maxGas, null, null, parameters).ConfigureAwait(false);
        }

        public async Task SubmitAnswerAsync(long questionId, Guid answerId, CancellationToken token)
        {
            var contract = await GetContractAsync(GenerateWeb3Instance("10f158cd550649e9f99e48a9c7e2547b65f101a2f928c3e0172e425067e51bb4"), token).ConfigureAwait(false);
            var operationToExe = contract.GetFunction("submitNewAnswer");
            await operationToExe.SendTransactionAndWaitForReceiptAsync(token, questionId, answerId.ToString()).ConfigureAwait(false);
        }

        public async Task MarkAsCorrectAsync(string userAddress, string winnerAddress, long question, Guid answerId, CancellationToken token)
        {
            var contract = await GetContractAsync(GenerateWeb3Instance("10f158cd550649e9f99e48a9c7e2547b65f101a2f928c3e0172e425067e51bb4"), token).ConfigureAwait(false);
            var operationToExe = contract.GetFunction("approveAnswer");
            var maxGas = new HexBigInteger(3000000);
            var parameters = new object[] { question, answerId.ToString(), winnerAddress, userAddress };
            await operationToExe.SendTransactionAndWaitForReceiptAsync(GetPublicAddress("10f158cd550649e9f99e48a9c7e2547b65f101a2f928c3e0172e425067e51bb4"), maxGas, null, null, parameters).ConfigureAwait(false);
        }

        

        public async Task UpVoteAsync(string userAddress, long question, Guid answerId, CancellationToken token)
        {
            var contract = await GetContractAsync(GenerateWeb3Instance("10f158cd550649e9f99e48a9c7e2547b65f101a2f928c3e0172e425067e51bb4"), token).ConfigureAwait(false);
            var operationToExe = contract.GetFunction("upVote");
            var maxGas = new HexBigInteger(3000000);
            var parameters = new object[] { question, answerId.ToString(), userAddress };
            await operationToExe.SendTransactionAndWaitForReceiptAsync(GetPublicAddress("10f158cd550649e9f99e48a9c7e2547b65f101a2f928c3e0172e425067e51bb4"), maxGas, null, null, parameters).ConfigureAwait(false);
        }

        public async Task<IEnumerable<string>> UpVoteListAsync(long questionId, Guid answerId, CancellationToken token)
        {
            var contract = await GetContractAsync(GenerateWeb3Instance(), token).ConfigureAwait(false);
            var function = contract.GetFunction("returnUpVoteList");
            return await function.CallAsync<List<string>>(questionId, answerId.ToString()).ConfigureAwait(false);
        }



        //private static Task<TransactionReceipt> SendTransactionAndWaitForReceiptAsync(CancellationToken receiptRequestCancellationToken, params object[] functionInput)
        //{
        //    //var maxGas = new HexBigInteger(3000000);
        //    //var publicAddress =
        //    //    BlockChainProvider.GetPublicAddress(
        //    //        "10f158cd550649e9f99e48a9c7e2547b65f101a2f928c3e0172e425067e51bb4");
        //    return SendTransactionAndWaitForReceiptAsync(receiptRequestCancellationToken,
        //        functionInput);
        //}

    }
}
