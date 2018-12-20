//using System;
//using System.Linq.Expressions;
//using Cloudents.Application.Interfaces;
//using Cloudents.Domain.Entities;

//namespace Cloudents.Application.Query
//{
//    public class UserDataExpressionQuery : IQuery<RegularUser>
//    {
//        public UserDataExpressionQuery(Expression<Func<RegularUser, bool>> expression)
//        {
//            QueryExpression = expression;
//        }

//        public Expression<Func<RegularUser, bool>> QueryExpression { get; }
//    }

//    public class UserLoginQuery : IQuery<RegularUser>
//    {
//        public UserLoginQuery(string loginProvider, string providerKey)
//        {
//            LoginProvider = loginProvider;
//            ProviderKey = providerKey;
//        }

//        public string LoginProvider { get; }
//        public string ProviderKey { get; }
//    }
//}