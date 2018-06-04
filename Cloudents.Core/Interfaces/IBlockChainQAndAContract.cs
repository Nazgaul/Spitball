﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IBlockChainQAndAContract
    {
        Task SubmitQuestionAsync(long questionId, decimal price, string userAddress, CancellationToken token);
        Task SubmitAnswerAsync(long questionId, Guid answerId, CancellationToken token);
        Task MarkAsCorrectAsync(string userAddress, string winnerAddress, long questionId, Guid answerId, CancellationToken token);
        Task UpVoteAsync(string userAddress, long questionId, Guid answerId, /*decimal price,*/ CancellationToken token);
        Task<IEnumerable<string>> UpVoteListAsync(long questionId, Guid answerId, CancellationToken token);
        Task SpreadFounds(long questionId, CancellationToken token);
    }
}
