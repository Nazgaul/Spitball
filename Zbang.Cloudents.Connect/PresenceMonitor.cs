using System;
using System.Linq;
using System.Threading;
using Microsoft.AspNet.SignalR.Transports;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Cloudents.Connect
{
    /// <summary>
    /// This class keeps track of connections that the <see cref="PresenceMonitor"/>
    /// has seen. It uses a time based system to verify if connections are *actually* still online.
    /// Using this class combined with the connection events SignalR raises will ensure
    /// that your database will always be in sync with what SignalR is seeing.
    /// </summary>
    public class PresenceMonitor : IDisposable
    {
        private readonly ITransportHeartbeat m_Heartbeat;
        private readonly IZboxWriteService m_ZboxWriteService;
        private Timer m_Timer;

        // How often we plan to check if the connections in our store are valid
        private readonly TimeSpan m_PresenceCheckInterval = TimeSpan.FromSeconds(15);

        // How many periods need pass without an update to consider a connection invalid
       // private const int PeriodsBeforeConsideringZombie = 3;

        // The number of seconds that have to pass to consider a connection invalid.
        //private readonly int m_ZombieThreshold;

        public PresenceMonitor(ITransportHeartbeat heartbeat, IZboxWriteService zboxWriteService)
        {
            m_Heartbeat = heartbeat;
            m_ZboxWriteService = zboxWriteService;
            //  m_ZombieThreshold = (int)m_PresenceCheckInterval.TotalSeconds * PeriodsBeforeConsideringZombie;
        }

        public void StartMonitoring()
        {
            if (m_Timer == null)
            {
                m_Timer = new Timer(_ =>
                {
                    try
                    {
                       Check();
                    }
                    catch(Exception ex)
                    {
                        // Don't throw on background threads, it'll kill the entire process
                        TraceLog.WriteWarning(ex.Message);
                    }
                },
                null,
                TimeSpan.Zero,
                m_PresenceCheckInterval);
            }
        }

        private void Check()
        {
            var connectionIds = m_Heartbeat.GetConnections().Where(w => w.IsAlive).Select(s => s.ConnectionId).ToList();
            if (!connectionIds.Any()) return;
            var command = new ManageConnectionsCommand(connectionIds);
            m_ZboxWriteService.ManageConnections(command);
        }

        public void Dispose()
        {
            m_Timer?.Dispose();
        }
    }
}
