using System;
using System.Linq.Expressions;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Query
{
    public class UserDataExpressionQuery : IQuery<User>
    {
        public UserDataExpressionQuery(Expression<Func<User, bool>> expression)
        {
            QueryExpression = expression;
        }

        public Expression<Func<User, bool>> QueryExpression { get; }
    }
}