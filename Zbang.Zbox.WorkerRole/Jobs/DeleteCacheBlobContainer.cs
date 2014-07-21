using System;
using System.Threading;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.WorkerRole.Jobs
{
    internal class DeleteCacheBlobContainer : IJob
    {

        private readonly IBlobProvider m_BlobProvider;
        private bool m_KeepRunning;

        public DeleteCacheBlobContainer(IBlobProvider blobProvider)
        {
            m_BlobProvider = blobProvider;
        }
        public void Run()
        {
            try
            {
                m_KeepRunning = true;
                while (m_KeepRunning)
                {
                    Execute();
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On Run DeleteCacheBlobContainer", ex);
                throw;
            }

        }

        private void Execute()
        {
          //  m_BlobProvider.DeleteCahceContent();
            Thread.Sleep(TimeSpan.FromDays(7));
        }
        public void Stop()
        {
            m_KeepRunning = false;
        }
    }
}
