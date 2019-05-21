using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;

namespace Cloudents.Query.Chat
{
    public class UserUnreadMessageQuery : IQuery<IList<UnreadMessageDto>>
    {
        public UserUnreadMessageQuery(byte[] version)
        {
            Version = version;
        }

        private byte[] Version { get; }

        internal sealed class UserUnreadMessageQueryHandler : IQueryHandler<UserUnreadMessageQuery, IList<UnreadMessageDto>>
        {
            private readonly QuerySession _querySession;

            public UserUnreadMessageQueryHandler(QuerySession querySession)
            {
                _querySession = querySession;
            }

            public async Task<IList<UnreadMessageDto>> GetAsync(UserUnreadMessageQuery query, CancellationToken token)
            {
                RegularUser userAlias = null;
                UnreadMessageDto resultAlias = null;

                var z = _querySession.StatelessSession.QueryOver<ChatUser>()
                        .JoinAlias(x => x.User, () => userAlias)
                        .Where(w => w.Unread > 0);
                if (query.Version != null)
                {
                    z.Where(Restrictions.Gt(nameof(ChatUser.Version), query.Version));
                }
                var result = await z.SelectList(s =>

                        s.Select(() => userAlias.PhoneNumber).WithAlias(() => resultAlias.PhoneNumber)
                            .Select(() => userAlias.Id).WithAlias(() => resultAlias.UserId)
                            .Select(x => x.Version).WithAlias(() => resultAlias.Version)
                     )
                    .TransformUsing(Transformers.AliasToBean<UnreadMessageDto>())
                     .ListAsync<UnreadMessageDto>(token);
             
                return result;
            }
        }
    }


    public class UnreadMessageDto
    {
        public string PhoneNumber { get; set; }
        public long UserId { get; set; }

        public byte[] Version { get; set; }

        public long VersionAsLong => BitConverter.ToInt64(Version.Reverse().ToArray(), 0);
    }
}