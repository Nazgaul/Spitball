using Cloudents.Core.Entities.Db;
using Cloudents.Core.Message.System;
using Cloudents.Core.Storage;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.Services
{
    public class QueueProfileUpdater : IProfileUpdater
    {
        private readonly IQueueProvider _queueProvider;
        private readonly SignInManager<User> _signInManager;

        public QueueProfileUpdater(IQueueProvider queueProvider, SignInManager<User> signInManager
            )
        {
            _queueProvider = queueProvider;
            _signInManager = signInManager;
        }


        public Task AddTagToUser(string tag, ClaimsPrincipal user, CancellationToken token)
        {
            if (string.IsNullOrEmpty(tag)) return Task.CompletedTask;
            if (!_signInManager.IsSignedIn(user)) return Task.CompletedTask;
            var userId = _signInManager.UserManager.GetLongUserId(user);

            return _queueProvider.InsertMessageAsync(new AddUserTagMessage(userId, tag), token);
        }
    }

    public interface IProfileUpdater
    {
        Task AddTagToUser(string tag, ClaimsPrincipal user, CancellationToken token);
    }
}
