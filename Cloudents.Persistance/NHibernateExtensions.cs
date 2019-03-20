﻿using NHibernate;

namespace Cloudents.Persistence
{
    public static class NHibernateExtensions
    {
        public static IQueryOver<TRoot, TSubType>
            OrderByRandom<TRoot, TSubType>(
                this IQueryOver<TRoot, TSubType> query)
        {
            query.UnderlyingCriteria.AddOrder(new RandomOrder());
            return query;
        }
    }
}