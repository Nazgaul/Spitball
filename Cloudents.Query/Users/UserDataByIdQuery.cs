﻿using Cloudents.Core.Entities;
using NHibernate;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Users
{
    public class UserDataByIdQuery : IQuery<User>

    {
        public UserDataByIdQuery(long id)
        {
            Id = id;
        }

        private long Id { get; }

        internal sealed class UserDataByIdQueryHandler : IQueryHandler<UserDataByIdQuery, User>
        {
            private readonly IStatelessSession _session;

            public UserDataByIdQueryHandler(IStatelessSession session)
            {
                _session = session;
            }

            public Task<User> GetAsync(UserDataByIdQuery query, CancellationToken token)
            {
                return _session.GetAsync<User>(query.Id, token);
            }
        }
    }
}