using Cloudents.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Infrastructure.BlockChain;
using System.Collections.Generic;
using System.Numerics;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Blockchain
{
    [UsedImplicitly]
    public class QAndAService : BlockChainProvider, IBlockChainQAndAContract
    {
        protected override string Abi => "QAndA";
        private const double Gas = 3e6;

        protected override string ContractAddress => "0xdfb4dd2a403d65dc3eb5f45058a8c2dd9f4092d9";

        public QAndAService (IConfigurationKeys configurationKeys) : base(configurationKeys)
        {
        }

        public async Task SubmitQuestionAsync(long questionId, decimal price, string userAddress,
            CancellationToken token)
        {
            var weiPrice = new BigInteger((double)price * FromWei);
            var operationToExe = await GetFunctionAsync("submitNewQuestion", token).ConfigureAwait(false);
            await operationToExe.SendTransactionAndWaitForReceiptAsync(SpitballPrivateKey, Gas, token, questionId, weiPrice, userAddress).ConfigureAwait(false);
        }

        public async Task SubmitAnswerAsync(long questionId, Guid answerId, CancellationToken token)
        {
            var operationToExe = await GetFunctionAsync("submitNewAnswer", token).ConfigureAwait(false);
            await operationToExe.SendTransactionAndWaitForReceiptAsync(SpitballPrivateKey, Gas, token, questionId, answerId.ToString()).ConfigureAwait(false);
        }

        public async Task MarkAsCorrectAsync(string userAddress, string winnerAddress, long questionId, Guid answerId, CancellationToken token)
        {
            var operationToExe = await GetFunctionAsync("approveAnswer", token).ConfigureAwait(false);
            await operationToExe.SendTransactionAndWaitForReceiptAsync(SpitballPrivateKey, Gas, token, questionId, answerId.ToString(), winnerAddress, userAddress).ConfigureAwait(false);
        }

        public async Task UpVoteAsync(string userAddress, long questionId, Guid answerId, CancellationToken token)
        {
            var operationToExe = await GetFunctionAsync("upVote", token).ConfigureAwait(false);
            await operationToExe.SendTransactionAndWaitForReceiptAsync(SpitballPrivateKey, Gas, token, questionId,
                answerId.ToString(), userAddress).ConfigureAwait(false);
        }

        public async Task<IEnumerable<string>> UpVoteListAsync(long questionId, Guid answerId, CancellationToken token)
        {
            var function = await GetFunctionAsync("returnUpVoteList", token).ConfigureAwait(false);
            return await function.CallAsync<List<string>>(questionId, answerId.ToString()).ConfigureAwait(false);
        }

        public async Task SpreadFounds(long questionId, CancellationToken token)
        {
            var operationToExe = await GetFunctionAsync("spreadFounds", token).ConfigureAwait(false);
            await operationToExe.SendTransactionAndWaitForReceiptAsync(SpitballPrivateKey, Gas, token, questionId).ConfigureAwait(false);
        }
    }
}
