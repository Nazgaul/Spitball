using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRole.DomainProcess
{
    class UpdatesProcess : IDomainProcess
    {
        private readonly IZboxWriteService m_ZboxWriteService;

        public UpdatesProcess(IZboxWriteService zboxService)
        {
            m_ZboxWriteService = zboxService;
        }


        public bool Excecute(Infrastructure.Transport.DomainProcess data)
        {
            var parameters = data as UpdateData;
            if (parameters == null) return true;
            try
            {
                m_ZboxWriteService.AddNewUpdate(new AddNewUpdatesCommand(
                    parameters.BoxId,
                    parameters.UserWhoMadeActionId,
                    parameters.QuestionId,
                    parameters.AnswerId,
                    parameters.ItemId,
                    parameters.QuizId
                    ));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On new update model:" + parameters, ex);
            }
            //var command = new UpdateStatisticsCommand(parameters4.ItemsIds.Select(s => new Zbang.Zbox.Domain.Commands.UpdateStatisticsCommand.StatisticItemData { ItemId = s.Id, Action = (StatisticsAction)s.Action }), parameters4.UserId, parameters4.StatTime);
            //m_ZboxWriteService.Statistics(command);

            return true;
        }
    }
}
