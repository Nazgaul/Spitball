using Cloudents.Core;
using Cloudents.Core.Extension;
using Cloudents.Query;
using Dapper;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppClaimsPrincipalFactory = Cloudents.Web.Identity.AppClaimsPrincipalFactory;

namespace Cloudents.Web.Hubs
{
    [Authorize]
    public class SbHub : Hub
    {
        private readonly Lazy<TelemetryClient> _logger;
        private readonly IDapperRepository _dapper;

        private static readonly ConnectionMapping<long> Connections =
            new ConnectionMapping<long>();


        private static bool _canUpdateDb = true;

        public SbHub(Lazy<TelemetryClient> logger, IDapperRepository dapper)
        {
            _logger = logger;
            _dapper = dapper;
        }

        public const string MethodName = "Message";
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync(MethodName, message);
        }

        public async Task Message(object entity)
        {
            await Clients.All.SendAsync(MethodName, entity);
        }


        private async Task ChangeOnlineStatus(long currentUserId, bool isOnline)
        {
            try
            {
                if (_canUpdateDb)
                {
                    const string sql = @"update sb.[user] set Online = @IsOnline, LastOnline = GETUTCDATE() where Id = @Id";
                    using (var connection = _dapper.OpenConnection())
                    {
                        await connection.ExecuteAsync(sql, new { Id = currentUserId, IsOnline = isOnline });
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Value.TrackException(e, new Dictionary<string, string>()
                {
                    ["SignalR"] = "Signalr",
                    ["currentUserId"] = "currentUserId"
                });
                _canUpdateDb = false;
            }
            var message = new SignalRTransportType(SignalRType.User, SignalREventAction.OnlineStatus,
                new
                {
                    id = currentUserId,
                    online = true
                });
            await Clients.All.SendAsync(MethodName, message);
        }

        public override async Task OnConnectedAsync()
        {
            var header = Context.GetHttpContext().Request.Headers["Origin"];
            var originsAllow = new[] { "localhost", "spitball", "azurewebsites", "trafficmanager" };

            var currentUserId = long.Parse(Context.UserIdentifier);
            if (!header.ToString().Contains(originsAllow, StringComparison.OrdinalIgnoreCase))
            {
                _logger.Value.TrackTrace("Investigate online ", SeverityLevel.Warning, new Dictionary<string, string>()
                {
                    ["userId"] = currentUserId.ToString(),
                    ["header"] = header
                });
            }
            //if (header.ToString().Contains("spi"))

            var country = Context.User.Claims.FirstOrDefault(f =>
                string.Equals(f.Type, AppClaimsPrincipalFactory.Country,
                    StringComparison.OrdinalIgnoreCase))?.Value;

            Connections.Add(currentUserId, Context.ConnectionId);
            var connectionCount = Connections.GetConnections(currentUserId).Count();
            if (connectionCount == 1)
            {
                await ChangeOnlineStatus(currentUserId, true);
            }
            else
            {
                _logger.Value.TrackTrace("Investigate online ", SeverityLevel.Warning, new Dictionary<string, string>()
                {
                    ["userId"] = currentUserId.ToString(),
                    ["connectionCount"] = connectionCount.ToString()
                });
                //_logger.Value.Warning($"Investigate online {currentUserId}");
            }

            if (country != null)
            {

                await Groups.AddToGroupAsync(Context.ConnectionId, $"country_{country.ToLowerInvariant()}");
            }

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var country = Context.User.Claims.FirstOrDefault(f =>
                string.Equals(f.Type, AppClaimsPrincipalFactory.Country,
                    StringComparison.OrdinalIgnoreCase))?.Value;
            if (country != null)
            {

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"country_{country.ToLowerInvariant()}");
            }

            var currentUserId = long.Parse(Context.UserIdentifier);
            Connections.Remove(currentUserId, Context.ConnectionId);

            var connectionCount = Connections.GetConnections(currentUserId).Count();

            if (connectionCount == 0)
            {
                await ChangeOnlineStatus(currentUserId, false);
            }
            else
            {
                _logger.Value.TrackTrace("Investigate offline ", SeverityLevel.Warning, new Dictionary<string, string>()
                {
                    ["userId"] = currentUserId.ToString(),
                    ["connectionCount"] = connectionCount.ToString()
                });
            }


        }

    }
}
