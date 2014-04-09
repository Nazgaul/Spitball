using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Common;
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
            if (parameters != null)
            {
                m_ZboxWriteService.AddNewUpdate(new Domain.Commands.AddNewUpdatesCommand(
                    parameters.BoxId,
                    parameters.UserWhoMadeActionId,
                    parameters.QuestionId,
                    parameters.AnswerId,
                    parameters.ItemId,
                    parameters.QuizId
                   ));
                //var command = new UpdateStatisticsCommand(parameters4.ItemsIds.Select(s => new Zbang.Zbox.Domain.Commands.UpdateStatisticsCommand.StatisticItemData { ItemId = s.Id, Action = (StatisticsAction)s.Action }), parameters4.UserId, parameters4.StatTime);
                //m_ZboxWriteService.Statistics(command);
            }

            return true;
        }
    }
}
