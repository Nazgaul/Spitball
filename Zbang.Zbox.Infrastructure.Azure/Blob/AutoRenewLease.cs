using System;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;

namespace Zbang.Zbox.Infrastructure.Azure.Blob
{
    public class AutoRenewLease : IDisposable
    {
        public bool HasLease => m_LeaseId != null;

        readonly AccessCondition m_AccessCondition;
        private readonly CloudBlockBlob m_Blob;
        private readonly string m_LeaseId;
        private Thread m_RenewalThread;
        private bool m_Disposed;

        public AutoRenewLease(CloudBlockBlob blob)
        {
            m_Blob = blob;
            blob.Container.CreateIfNotExists();
            try
            {
                if (!blob.Exists())
                {
                    blob.UploadFromByteArray(new byte[0], 0, 0, AccessCondition.GenerateIfNoneMatchCondition("*"));// new BlobRequestOptions { AccessCondition = AccessCondition.IfNoneMatch("*") });
                }
            }
            catch (StorageException e)
            {
                if (e.RequestInformation.HttpStatusCode != (int)HttpStatusCode.PreconditionFailed // 412 from trying to modify a blob that's leased
                    && e.RequestInformation.ExtendedErrorInformation.ErrorCode != BlobErrorCodeStrings.BlobAlreadyExists
                    )
                {
                    throw;
                }
            }
            try
            {
                m_LeaseId = blob.AcquireLease(TimeSpan.FromSeconds(60), null);
                m_AccessCondition = new AccessCondition { LeaseId = m_LeaseId };
            }
            catch (Exception)
            {
                System.Diagnostics.Trace.WriteLine("==========> Lease rejected! <==========");
            }

            if (!HasLease) return;
            m_RenewalThread = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(40));
                    var ac = new AccessCondition {LeaseId = m_LeaseId};
                    blob.RenewLease(ac);//.RenewLease(leaseId);
                }
            });
            m_RenewalThread.Start();
        }

        public static void DoOnce(CloudBlockBlob blob, Action action) { DoOnce(blob, action, TimeSpan.FromSeconds(5)); }
        public static void DoOnce(CloudBlockBlob blob, Action action, TimeSpan pollingFrequency)
        {
            // blob.Exists has the side effect of calling blob.FetchAttributes, which populates the metadata collection
            while (!blob.Exists() || blob.Metadata["progress"] != "done")
            {
                using (var arl = new AutoRenewLease(blob))
                {
                    if (arl.HasLease)
                    {
                        action();
                        blob.Metadata["progress"] = "done";
                        AccessCondition ac = new AccessCondition {LeaseId = arl.m_LeaseId};
                        blob.SetMetadata(ac);
                    }
                    else
                    {
                        Thread.Sleep(pollingFrequency);
                    }
                }
            }
        }

        
        public static void DoConsequence(string lockBlobName, Action action,
            string cnStrName = "StorageConnectionString",
            string containerName = "leasesContainer", TimeSpan? pollingFrequency = null)
        {
            //http://www.windowsazure.com/en-us/develop/net/how-to-guides/blob-storage/

           
            var account = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings[cnStrName].ConnectionString); //CloudStorageAccount.Parse("UseDevelopmentStorage=true"); // Не работает на SDK 2.2 // or your real connection string
            var blobs = account.CreateCloudBlobClient();
            var container = blobs
                .GetContainerReference(ConfigurationManager.AppSettings[containerName]);
            container.CreateIfNotExists();

            var blob = container.GetBlockBlobReference(lockBlobName);

            bool jobDone = false;

            while (!jobDone)
            {
                using (var arl = new AutoRenewLease(blob))
                {
                    if (arl.HasLease)
                    {
                        // Some Sync Work here 
                        action();
                        jobDone = true;
                    }
                    else
                    {
                        Thread.Sleep(pollingFrequency ?? TimeSpan.FromMilliseconds(300));
                    }
                }
            }
        }

        public static void DoEvery(CloudBlockBlob blob, TimeSpan interval, Action action)
        {
            while (true)
            {
                var lastPerformed = DateTimeOffset.MinValue;
                using (var arl = new AutoRenewLease(blob))
                {
                    if (arl.HasLease)
                    {
                        blob.FetchAttributes();
                        DateTimeOffset.TryParseExact(blob.Metadata["lastPerformed"], "R", CultureInfo.CurrentCulture, DateTimeStyles.AdjustToUniversal, out lastPerformed);
                        if (DateTimeOffset.UtcNow >= lastPerformed + interval)
                        {
                            action();
                            lastPerformed = DateTimeOffset.UtcNow;
                            blob.Metadata["lastPerformed"] = lastPerformed.ToString("R");
                            AccessCondition ac = new AccessCondition();
                            ac.LeaseId = arl.m_LeaseId;
                            blob.SetMetadata(ac);
                        }
                    }
                }
                var timeLeft = (lastPerformed + interval) - DateTimeOffset.UtcNow;
                var minimum = TimeSpan.FromSeconds(5); // so we're not polling the leased blob too fast
                Thread.Sleep(
                    timeLeft > minimum
                    ? timeLeft
                    : minimum);
            }
        }

        

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!m_Disposed)
            {
                if (disposing)
                {
                    if (m_RenewalThread != null)
                    {
                        m_RenewalThread.Abort();
                        m_Blob.ReleaseLease(m_AccessCondition);
                        m_RenewalThread = null;
                    }
                }
                m_Disposed = true;
            }
        }

        ~AutoRenewLease()
        {
            Dispose(false);
        }
    }
}