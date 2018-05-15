using System;
using NHibernate;

namespace Cloudents.Infrastructure.Database
{
    public interface IUnitOfWorkFactory
    {
        ISession OpenSession();
    }
}