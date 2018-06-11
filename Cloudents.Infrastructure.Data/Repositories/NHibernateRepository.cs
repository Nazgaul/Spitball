﻿using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Cloudents.Core.Attributes;
using Cloudents.Core.Interfaces;
using NHibernate;

namespace Cloudents.Infrastructure.Data.Repositories
{
    public class NHibernateRepository<T> : IRepository<T> where T : class
    {
        protected readonly ISession Session;
        private readonly IUnitOfWork _unitOfWork;

        public NHibernateRepository(IIndex<Core.Enum.Database, IUnitOfWork> unitOfWorks)
        {
            var att = typeof(T).GetCustomAttribute<DbAttribute>();
            _unitOfWork = unitOfWorks[att?.Database ?? Core.Enum.Database.System];
            Session = _unitOfWork.Session;
        }

        public Task<T> LoadAsync(object id, CancellationToken token)
        {
            return Session.LoadAsync<T>(id, token);
        }

        public Task<T> GetAsync(object id, CancellationToken token)
        {
            return Session.GetAsync<T>(id, token);
        }


        public Task DeleteAsync(object id, CancellationToken token)
        {
            return Session.DeleteAsync(id, token);
        }

        public IQueryable<T> GetQueryable()
        {
           return Session.Query<T>();
        }

        public Task<object> SaveAsync(T entity, CancellationToken token)
        {
            _unitOfWork.FlagCommit();
            return Session.SaveAsync(entity, token);
        }

        public Task SaveOrUpdateAsync(T entity, CancellationToken token)
        {
            _unitOfWork.FlagCommit();
            return Session.SaveOrUpdateAsync(entity, token);
        }
    }
}
