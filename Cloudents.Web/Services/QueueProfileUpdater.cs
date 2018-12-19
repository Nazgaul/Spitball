using Cloudents.Domain.Entities;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Message.System;
using Cloudents.Application.Storage;

namespace Cloudents.Web.Services
{
    public class QueueProfileUpdater : IProfileUpdater
    {
        private readonly IQueueProvider _queueProvider;
        private readonly SignInManager<RegularUser> _signInManager;

        public QueueProfileUpdater(IQueueProvider queueProvider, SignInManager<RegularUser> signInManager
            )
        {
            _queueProvider = queueProvider;
            _signInManager = signInManager;
        }


        public Task AddTagToUser(string tag, ClaimsPrincipal user, CancellationToken token)
        {
            if (!_signInManager.IsSignedIn(user)) return Task.CompletedTask;
            if (Tag.ValidateTag(tag))
            {

                var userId = _signInManager.UserManager.GetLongUserId(user);

                return _queueProvider.InsertMessageAsync(new AddUserTagMessage(userId, tag), token);
            }

            return Task.CompletedTask;
        }
    }

    public interface IProfileUpdater
    {
        Task AddTagToUser(string tag, ClaimsPrincipal user, CancellationToken token);
    }
}
