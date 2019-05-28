using System;
using System.Collections.Generic;
using System.Globalization;
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
                ChatUser chatUserAlias = null;
                UnreadMessageDto resultAlias = null;
                Core.Entities.Tutor tutorAlias = null;

                var z = _querySession.StatelessSession.QueryOver<ChatUser>(() => chatUserAlias)
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
                            .Select(() => userAlias.Language).WithAlias(() => resultAlias.CultureInfo)
                            
                            .SelectSubQuery(QueryOver.Of<ChatMessage>()
                                .Where(w=>w.ChatRoom.Id == chatUserAlias.ChatRoom.Id)
                                .Select(s1 => s1.User.Id)
                                .OrderBy(o => o.Id).Asc
                                .Take(1)
                                
                                ).WithAlias(() => resultAlias.ChatUserId)
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

        public CultureInfo CultureInfo { get; set; }

        public bool IsTutor => UserId != ChatUserId;

        public long ChatUserId { get; set; }

        public long VersionAsLong => BitConverter.ToInt64(Version.Reverse().ToArray(), 0);
    }
}