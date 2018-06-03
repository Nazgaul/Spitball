﻿using Cloudents.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Infrastructure.BlockChain;
using System.Collections.Generic;
using JetBrains.Annotations;
using Nethereum.Contracts;

namespace Cloudents.Infrastructure.Blockchain
{
    [UsedImplicitly]
    public class QAndAService : BlockChainProvider, IBlockChainQAndAContract
    {
        protected override string Abi => "QAndA";
        private const double Gas = 3e6;

        protected override string TransactionHash => "0xa976e2a5828a8a53d0f1f76d931613ee40cc0281e977a658a442873a379e1f66";

        public QAndAService (IConfigurationKeys configurationKeys) : base(configurationKeys)
        {
        }

        public async Task SubmitQuestionAsync(long questionId, decimal price, string userAddress,
            CancellationToken token)
        {
            var operationToExe = await GetFunctionAsync("submitNewQuestion", token).ConfigureAwait(false);
            await operationToExe.SendTransactionAndWaitForReceiptAsync(SpitballPrivateKey, Gas, token, questionId, price, userAddress).ConfigureAwait(false);
        }

        

        public async Task SubmitAnswerAsync(long questionId, Guid answerId, CancellationToken token)
        {
            var operationToExe = await GetFunctionAsync("submitNewAnswer", token).ConfigureAwait(false);
            await operationToExe.SendTransactionAndWaitForReceiptAsync(SpitballPrivateKey, Gas, token, questionId, answerId.ToString()).ConfigureAwait(false);
        }

        public async Task MarkAsCorrectAsync(string userAddress, string winnerAddress, long question, Guid answerId, CancellationToken token)
        {
            var operationToExe = await GetFunctionAsync("approveAnswer", token).ConfigureAwait(false);
            await operationToExe.SendTransactionAndWaitForReceiptAsync(SpitballPrivateKey, Gas, token, question, answerId.ToString(), winnerAddress, userAddress).ConfigureAwait(false);
        }

        public async Task UpVoteAsync(string userAddress, long question, Guid answerId, CancellationToken token)
        {
            var operationToExe = await GetFunctionAsync("upVote", token).ConfigureAwait(false);
            await operationToExe.SendTransactionAndWaitForReceiptAsync(SpitballPrivateKey, Gas, token, question,
                answerId.ToString(), userAddress).ConfigureAwait(false);
        }

        public async Task<IEnumerable<string>> UpVoteListAsync(long questionId, Guid answerId, CancellationToken token)
        {
            var function = await GetFunctionAsync("returnUpVoteList", token).ConfigureAwait(false);
            return await function.CallAsync<List<string>>(questionId, answerId.ToString()).ConfigureAwait(false);
        }

        private async Task<Function> GetFunctionAsync(string name, CancellationToken token)
        {
            var contract = await GetContractAsync(GenerateWeb3Instance(SpitballPrivateKey), token).ConfigureAwait(false);
            return contract.GetFunction(name);
        }

    }
}
