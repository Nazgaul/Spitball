using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Chat;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure
{
    public class ChatProvider : IChat
    {
        private readonly IRestClient _client;

        public ChatProvider(IRestClient client)
        {
            _client = client;
        }

        public Task CreateOrUpdateUserAsync(long id, User user, CancellationToken token)
        {
            return _client.PutJsonAsync(
                new Uri($"https://api.talkjs.com/v1/tXsrQpOx/users/{id}"),
                user, null, token);
        }
    }
}