using System;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch.DomainProcess
{
    internal class UpdatesProcess : IDomainProcess
    {
        private readonly IZboxWorkerRoleService m_ZboxWriteService;

        public UpdatesProcess(IZboxWorkerRoleService zboxService)
        {
            m_ZboxWriteService = zboxService;
        }


        public async Task<bool> ExecuteAsync(Infrastructure.Transport.DomainProcess data, CancellationToken token)
        {
            var parameters = data as UpdateData;
            if (parameters == null) return true;
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
                   parameters.QuizDiscussionId)
               ).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On new update model: " + parameters, ex);
                return false;
            }
            return true;
        }
    }
}
