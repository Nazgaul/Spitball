﻿using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Web.Identity;
using Cloudents.Core.Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.ServiceBus;
using Cloudents.Query;
using Dapper;

namespace Cloudents.Web.Hubs
{
    [Authorize]
    public class SbHub : Hub
    {
        private readonly Lazy<ICommandBus> _commandBus;
        private readonly Lazy<ILogger> _logger;
        private readonly DapperRepository _dapper;

        private static readonly ConnectionMapping<long> _connections =
            new ConnectionMapping<long>();


        private static bool _canUpdateDb = true;

        public SbHub(Lazy<ICommandBus> commandBus, Lazy<ILogger> logger, DapperRepository dapper)
        {
            _commandBus = commandBus;
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


        private async Task ChangeOnlineStatus(long currentUserId, bool isTurnOn)
        {
            _logger.Value.Info($"current user online {currentUserId}");
            try
            {
                if (_canUpdateDb)
                {
                    var sql = @"update sb.[user] set Online = @IsOnline, LastOnline = GETUTCDATE() where Id = @Id
	                        and (LockoutEnd is null or LockoutEnd < GETUTCDATE())";
                    int rows;
                    using (var connection = _dapper.OpenConnection())
                    {
                        rows = await connection.ExecuteAsync(sql, new { Id = currentUserId, IsOnline = isTurnOn });
                    }

                    if (rows == 0)
                    {
                        _logger.Value.Exception(new UserLockoutException(), new Dictionary<string, string>()
                        {
                            ["SignalR"] = "Signalr",
                            ["currentUserId"] = "currentUserId"
                        });
                        var message2 = new SignalRTransportType(SignalRType.User, SignalREventAction.Logout,
                            new object());
                        await Clients.User(Context.UserIdentifier).SendCoreAsync("Message", new object[]
                        {
                        message2
                        });
                        return;
                    }
                }

            }
            catch (Exception e)
            {
                _logger.Value.Exception(e, new Dictionary<string, string>()
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
                _logger.Value.Error($"Investigate online {currentUserId}");
            }
            //if (header.ToString().Contains("spi"))

            var country = Context.User.Claims.FirstOrDefault(f =>
                string.Equals(f.Type, AppClaimsPrincipalFactory.Country.ToString(),
                    StringComparison.OrdinalIgnoreCase))?.Value;

            _connections.Add(currentUserId, Context.ConnectionId);
            var connectionCount = _connections.GetConnections(currentUserId).Count();
            //if (connectionCount > 5)
            //{
            //    _logger.Value.Warning($"Investigate online {currentUserId}");
            //}
            if (connectionCount == 1)
            {
                await ChangeOnlineStatus(currentUserId, true);
            }
            else
            {
                _logger.Value.Warning($"Investigate online {currentUserId}");
            }
            //try
            //{

            //    if (result)
            //    {
            //        _logger.Value.Info($"Need to change online status true of user {currentUserId}");
            //       // var command = new ChangeOnlineStatusCommand(currentUserId, true);
            //       // await _commandBus.Value.DispatchAsync(command, default);
            //    }


            //}
            //catch (Exception e)
            //{

            //    _logger.Value.Exception(e, new Dictionary<string, string>()
            //    {
            //        ["SignalR"] = "Signalr"
            //    });
            //}
            if (country != null)
            {

                await Groups.AddToGroupAsync(Context.ConnectionId, $"country_{country.ToLowerInvariant()}");
            }

            //var message = new SignalRTransportType(SignalRType.User, SignalREventAction.OnlineStatus,
            //    new
            //    {
            //        id = currentUserId,
            //        online = true
            //    });



            //await Clients.All.SendAsync(MethodName, message);


            //  await Task.WhenAll(t2);
            //  await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var country = Context.User.Claims.FirstOrDefault(f =>
                string.Equals(f.Type, AppClaimsPrincipalFactory.Country.ToString(),
                    StringComparison.OrdinalIgnoreCase))?.Value;
            if (country != null)
            {

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"country_{country.ToLowerInvariant()}");
            }

            var currentUserId = long.Parse(Context.UserIdentifier);
            _connections.Remove(currentUserId, Context.ConnectionId);

            var connectionCount = _connections.GetConnections(currentUserId).Count();

            if (connectionCount == 0)
            {
                await ChangeOnlineStatus(currentUserId, false);
            }
            else
            {
                _logger.Value.Warning($"Investigate offline {currentUserId}");
            }


        }

    }
}
