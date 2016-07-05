using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Transports;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Cloudents.Connect
{
    /// <summary>
    /// This class keeps track of connections that the <see cref="UserTrackingHub"/>
    /// has seen. It uses a time based system to verify if connections are *actually* still online.
    /// Using this class combined with the connection events SignalR raises will ensure
    /// that your database will always be in sync with what SignalR is seeing.
    /// </summary>
    public class PresenceMonitor
    {
        private readonly ITransportHeartbeat m_Heartbeat;
        private readonly IQueueProvider m_QueueProvider;
        private Timer m_Timer;

        // How often we plan to check if the connections in our store are valid
        private readonly TimeSpan m_PresenceCheckInterval = TimeSpan.FromSeconds(10);

        // How many periods need pass without an update to consider a connection invalid
       // private const int PeriodsBeforeConsideringZombie = 3;

        // The number of seconds that have to pass to consider a connection invalid.
        //private readonly int m_ZombieThreshold;

        public PresenceMonitor(ITransportHeartbeat heartbeat, IQueueProvider queueProvider)
        {
            m_Heartbeat = heartbeat;
            m_QueueProvider = queueProvider;
          //  m_ZombieThreshold = (int)m_PresenceCheckInterval.TotalSeconds * PeriodsBeforeConsideringZombie;
        }

        public void StartMonitoring()
        {
           
            if (m_Timer == null)
            {
                m_Timer = new Timer(async _ =>
                {
                    try
                    {
                       await CheckAsync();
                    }
                    catch (Exception ex)
                    {
                        // Don't throw on background threads, it'll kill the entire process
                        Trace.TraceError(ex.Message);
                    }
                },
                null,
                TimeSpan.Zero,
                m_PresenceCheckInterval);
            }
        }

        private async Task CheckAsync()
        {
            //using (var db = new UserContext())
            //{
            // Get all connections on this node and update the activity
            var connectionIds = m_Heartbeat.GetConnections().Where(w => w.IsAlive).Select(s => s.ConnectionId).ToList();
            if (connectionIds.Any())
            {
                await m_QueueProvider.InsertMessageToThumbnailAsync(new SignalrConnectionsData(connectionIds));
            }
            //var command = new ManageConnectionsCommand(connectionIds);
            //var result = m_ZboxWriteService.ManageConnections(command);
            //var tasks = new List<Task>();
            //foreach (var connectionId in result.ConnectionIds)
            //{
            //   var connection = m_Heartbeat.GetConnections().FirstOrDefault(w => w.ConnectionId == connectionId);
            //    if (connection != null)
            //    {
            //        tasks.Add(connection.Disconnect());
            //    }
            //}
            //await Task.WhenAll(tasks);

        }
    }
}
