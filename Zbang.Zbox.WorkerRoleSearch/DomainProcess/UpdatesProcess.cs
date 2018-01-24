using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Cloudents.Core.Interfaces;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch.DomainProcess
{
    internal class UpdatesProcess : IDomainProcess
    {
        private readonly IZboxWorkerRoleService m_ZboxWriteService;
        private readonly ILogger _logger;

        public UpdatesProcess(IZboxWorkerRoleService zboxService, ILogger logger)
        {
            m_ZboxWriteService = zboxService;
            _logger = logger;
        }


        public async Task<bool> ExecuteAsync(Infrastructure.Transport.DomainProcess data, CancellationToken token)
        {
            if (!(data is UpdateData parameters)) return true;
            try
            {
               await m_ZboxWriteService.AddNewUpdateAsync(new AddNewUpdatesCommand(
                   parameters.BoxId,
                   parameters.UserWhoMadeActionId,
                   parameters.QuestionId,
                   parameters.AnswerId,
                   parameters.ItemId,
                   parameters.QuizId,
                   parameters.ItemDiscussionId,
                   parameters.ItemDiscussionReplyId,
                   parameters.QuizDiscussionId,
                   parameters.FlashcardId)
               ).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Exception(ex, new Dictionary<string, string>
                {
                    ["model"] = parameters.ToString()
                });
                return false;
            }
            return true;
        }
    }
}
