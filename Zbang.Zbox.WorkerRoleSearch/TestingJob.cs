﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Azure.Queue;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.WorkerRoleSearch.DomainProcess;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class TestingJob : IJob
    {
        private readonly IZboxWorkerRoleService m_ZboxWorkerRoleService;
        private readonly IZboxReadServiceWorkerRole m_ZboxReadService;
        private readonly IMailComponent m_MailComponent;
        private readonly IQueueProviderExtract m_QueueProvider;
        private readonly ILifetimeScope m_LifetimeScope;
        private readonly IBlobProvider2<FilesContainerName> m_BlobProvider;


        public TestingJob(IZboxWorkerRoleService zboxWorkerRoleService, IMailComponent mailComponent, IQueueProviderExtract queueProvider, IZboxReadServiceWorkerRole zboxReadService, ILifetimeScope lifetimeScope, IBlobProvider2<FilesContainerName> blobProvider)
        {
            m_ZboxWorkerRoleService = zboxWorkerRoleService;
            m_MailComponent = mailComponent;
            m_QueueProvider = queueProvider;
            m_ZboxReadService = zboxReadService;
            m_LifetimeScope = lifetimeScope;
            m_BlobProvider = blobProvider;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            TraceLog.WriteInfo("one time job starting to work");
            //while ((bytesReceived = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) > 0)
            IEnumerable<Tuple<long, string>> documents;
            var lastId = 0L;
            while ((documents = (await m_ZboxReadService.GetDocumentsWithoutMd5Async(lastId).ConfigureAwait(false)).ToList()).Any())
            {
                if (documents.Any(a => a.Item1 == lastId))
                {
                    TraceLog.WriteError("one time job doing duplicates");
                    break;
                }
                TraceLog.WriteInfo("one time job process batch");
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                foreach (var document in documents)
                {
                    TraceLog.WriteInfo($"one time job process id: {document.Item1}");
                    lastId = document.Item1;
                    var md5 = await m_BlobProvider.Md5Async(document.Item2).ConfigureAwait(false);
                    var command = new UpdateThumbnailCommand(document.Item1, null,
                        null, md5);
                    m_ZboxWorkerRoleService.UpdateThumbnailPicture(command);
                }
            }
            TraceLog.WriteInfo("one time job stop to work");
        }

        public void Stop()
        {
            // throw new NotImplementedException();
        }
    }
}
