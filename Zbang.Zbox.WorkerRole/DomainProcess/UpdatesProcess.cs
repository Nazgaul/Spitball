﻿using System;
using System.Threading.Tasks;
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


        public Task<bool> ExecuteAsync(Infrastructure.Transport.DomainProcess data)
        {
            var parameters = data as UpdateData;
            if (parameters == null) return Task.FromResult(true);
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
            return Task.FromResult(true);
        }
    }
}
