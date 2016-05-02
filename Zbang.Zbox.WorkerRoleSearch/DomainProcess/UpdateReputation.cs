﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch.DomainProcess
{
    public class UpdateReputation : IDomainProcess
    {
        private readonly IZboxWorkerRoleService m_ZboxWriteService;

        public UpdateReputation(IZboxWorkerRoleService zboxWriteService)
        {
            m_ZboxWriteService = zboxWriteService;
        }

        public Task<bool> ExecuteAsync(Infrastructure.Transport.DomainProcess data, CancellationToken token)
        {
            var parameters = data as ReputationData;
            if (parameters == null) return Infrastructure.Extensions.TaskExtensions.CompletedTaskTrue; 
            try
            {
                m_ZboxWriteService.UpdateReputation(new UpdateReputationCommand(parameters.UserIds));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On new update model:" + parameters, ex);
            }
            return Infrastructure.Extensions.TaskExtensions.CompletedTaskTrue;
        }
    }
}
