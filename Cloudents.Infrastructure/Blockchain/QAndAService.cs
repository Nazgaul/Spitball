using Cloudents.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Infrastructure.BlockChain;
using System.Collections.Generic;
using System.Numerics;
using Cloudents.Core.Request;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Blockchain
{
    [UsedImplicitly]
    public class QAndAService : BlockChainProvider, IBlockChainQAndAContract
    {
        protected override string Abi => "QAndA";
        private const double Gas = 3e6;

        protected override string ContractAddress => "0xd1569828528dfea142bebbaca86928c4ee4ba3f6";

        public QAndAService (IConfigurationKeys configurationKeys) : base(configurationKeys)
        {
        }

        public async Task SubmitAsync(BlockChainSubmitQuestion model, CancellationToken token)
        {
            var weiPrice = new BigInteger((double)model.Price * FromWei);
            var operationToExe = await GetFunctionAsync("submitNewQuestion", token).ConfigureAwait(false);
            await operationToExe.SendTransactionAndWaitForReceiptAsync(SpitballPrivateKey, Gas, token, model.QuestionId, weiPrice, model.UserAddress).ConfigureAwait(false);
        }

        public async Task SubmitAsync(BlockChainSubmitAnswer model, CancellationToken token)
        {
            var operationToExe = await GetFunctionAsync("submitNewAnswer", token).ConfigureAwait(false);
            await operationToExe.SendTransactionAndWaitForReceiptAsync(SpitballPrivateKey, Gas, token, model.QuestionId, model.SubmiterAddress, model.AnswerId.ToString()).ConfigureAwait(false);
        }

        public async Task SubmitAsync(BlockChainMarkQuestionAsCorrect model, CancellationToken token)
        {
            var operationToExe = await GetFunctionAsync("approveAnswer", token).ConfigureAwait(false);
            await operationToExe.SendTransactionAndWaitForReceiptAsync(SpitballPrivateKey, Gas, token, model.QuestionId, model.AnswerId.ToString(), model.WinnerAddress, model.UserAddress).ConfigureAwait(false);
        }

        //public async Task UpVoteAsync(string userAddress, long questionId, Guid answerId, CancellationToken token)
        //{
        //    var operationToExe = await GetFunctionAsync("upVote", token).ConfigureAwait(false);
        //    await operationToExe.SendTransactionAndWaitForReceiptAsync(SpitballPrivateKey, Gas, token, questionId,
        //        answerId.ToString(), userAddress).ConfigureAwait(false);
        //}

        //public async Task MarkAsCorrectAsync(string userAddress, string winnerAddress, long questionId, Guid answerId, CancellationToken token)
        //{
        //    var operationToExe = await GetFunctionAsync("approveAnswer", token).ConfigureAwait(false);
        //    await operationToExe.SendTransactionAndWaitForReceiptAsync(SpitballPrivateKey, Gas, token, questionId, answerId.ToString(), winnerAddress, userAddress).ConfigureAwait(false);
        //}

        public async Task SubmitAsync(BlockChainUpVote model, CancellationToken token)
        {
            var operationToExe = await GetFunctionAsync("upVote", token).ConfigureAwait(false);
            await operationToExe.SendTransactionAndWaitForReceiptAsync(SpitballPrivateKey, Gas, token, model.QuestionId, model.AnswerId.ToString(), model.UserAddress).ConfigureAwait(false);
        }

        public async Task<IEnumerable<string>> UpVoteListAsync(long questionId, Guid answerId, CancellationToken token)
        {
            var function = await GetFunctionAsync("returnUpVoteList", token).ConfigureAwait(false);
            return await function.CallAsync<List<string>>(questionId, answerId.ToString()).ConfigureAwait(false);
        }

        public async Task SpreadFoundsAsync(long questionId, CancellationToken token)
        {
            var operationToExe = await GetFunctionAsync("spreadFounds", token).ConfigureAwait(false);
            await operationToExe.SendTransactionAndWaitForReceiptAsync(SpitballPrivateKey, Gas, token, questionId).ConfigureAwait(false);
        }
    }
}
