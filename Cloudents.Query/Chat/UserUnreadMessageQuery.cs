﻿using Cloudents.Core.Entities;
using NHibernate.Criterion;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NHibernate;

namespace Cloudents.Query.Chat
{
    public class UserUnreadMessageQuery : IQuery<IList<UnreadMessageDto>>
    {
        public UserUnreadMessageQuery(byte[]? version)
        {
            Version = version;
        }

        private byte[]? Version { get; }

        internal sealed class UserUnreadMessageQueryHandler : IQueryHandler<UserUnreadMessageQuery, IList<UnreadMessageDto>>
        {
            private readonly IStatelessSession _querySession;

            public UserUnreadMessageQueryHandler(IStatelessSession querySession)
            {
                _querySession = querySession;
            }

            public async Task<IList<UnreadMessageDto>> GetAsync(UserUnreadMessageQuery query, CancellationToken token)
            {
                User? userAlias = null!;
                ChatUser? chatUserAlias = null!;
                UnreadMessageDto? resultAlias = null!;
                ChatRoom chatRoomAlias = null!;

                var z = _querySession.QueryOver(() => chatUserAlias)
                    
                        .JoinAlias(x => x.User, () => userAlias)
                        .JoinAlias(x=>x.ChatRoom, () => chatRoomAlias)
                        .Where(w => w.Unread > 0)
                        .And(() => userAlias.Email != null);
                if (query.Version != null)
                {
                    z.Where(Restrictions.Gt(nameof(ChatUser.Version), query.Version));
                }
                var result = await z.SelectList(s =>

                        //s.Select(() => userAlias.PhoneNumber).WithAlias(() => resultAlias.PhoneNumber)
                        s.Select(() => userAlias.Email).WithAlias(() => resultAlias.Email)
                            .Select(() => userAlias.Id).WithAlias(() => resultAlias.UserId)
                            .Select(x => x.Version).WithAlias(() => resultAlias.Version)
                            .Select(() => userAlias.Language).WithAlias(() => resultAlias.CultureInfo)
                            .Select(() => userAlias.SbCountry).WithAlias(() => resultAlias.Country)
                            .Select(() => chatRoomAlias.Identifier).WithAlias(() => resultAlias.Identifier)
                            .SelectSubQuery(QueryOver.Of<ChatMessage>()
                                .Where(w => w.ChatRoom.Id == chatUserAlias.ChatRoom.Id)
                                .ToRowCountQuery()
                            ).WithAlias(() => resultAlias.ChatMessagesCount)
                            .SelectSubQuery(QueryOver.Of<ChatMessage>()
                                .Where(w => w.ChatRoom.Id == chatUserAlias.ChatRoom.Id)
                                .Select(s1 => s1.User.Id)
                                .OrderBy(o => o.Id).Asc
                                .Take(1)

                                ).WithAlias(() => resultAlias.ChatUserId)
                     )
                    
                    .TransformUsing(Transformers.AliasToBean<UnreadMessageDto>())
                    .UnderlyingCriteria.SetComment(nameof(UserUnreadMessageQuery))
                     .ListAsync<UnreadMessageDto>(token);

                return result;
            }
        }
    }


    public class UnreadMessageDto
    {
        public override string ToString()
        {
            return $"{nameof(UserId)}: {UserId}, {nameof(CultureInfo)}: {CultureInfo}, {nameof(Country)}: {Country}, {nameof(IsTutor)}: {IsTutor}, {nameof(ChatUserId)}: {ChatUserId}, {nameof(ChatMessagesCount)}: {ChatMessagesCount}";
        }

        public string Email{ get; set; } = null!;

        //public string PhoneNumber { get; set; }
        public long UserId { get; set; }

        public byte[] Version { get; set; } = null!;

        public CultureInfo CultureInfo { get; set; }

        public Country? Country { get; set; }

        public bool IsTutor => UserId != ChatUserId;

        public long ChatUserId { get; set; }

        public int ChatMessagesCount { get; set; }

        public long VersionAsLong => BitConverter.ToInt64(Version.Reverse().ToArray(), 0);

      

        private sealed class UserIdEqualityComparer : IEqualityComparer<UnreadMessageDto>
        {
            public bool Equals(UnreadMessageDto x, UnreadMessageDto y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.UserId == y.UserId;
            }

            public int GetHashCode(UnreadMessageDto obj)
            {
                return obj.UserId.GetHashCode();
            }
        }

        public static IEqualityComparer<UnreadMessageDto> UserIdComparer { get; } = new UserIdEqualityComparer();
        public string Identifier { get; set; }
    }
}