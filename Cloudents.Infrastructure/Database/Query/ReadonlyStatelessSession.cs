﻿using System;
using System.Data;
using NHibernate;

namespace Cloudents.Infrastructure.Database.Query
{
    public sealed class ReadonlyStatelessSession : IDisposable
    {
        public IStatelessSession Session { get; }

        private readonly ITransaction _transaction;

        public ReadonlyStatelessSession(IStatelessSession session)
        {
            Session = session;
            _transaction = Session.BeginTransaction(IsolationLevel.ReadUncommitted);
        }

        public void Dispose()
        {
            _transaction?.Commit();
            _transaction?.Dispose();
            Session?.Dispose();
        }
    }
}