using NHibernate;
using NHibernate.Dialect;
using NHibernate.Dialect.Function;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using System;
using System.Collections.ObjectModel;
using System.Reflection;
using Cloudents.Query.Stuff;

namespace Cloudents.Persistence
{
    [Serializable]
    public class SbDialect : MsSql2012Dialect
    {
        internal const string RandomOrder = "random_Order";

        protected override void RegisterFunctions()
        {
            base.RegisterFunctions();
            RegisterFunction("contains", new StandardSQLFunction("contains", null));
            //RegisterFunction("string_agg", new StandardSQLFunction("string_agg", NHibernateUtil.String));

            RegisterFunction(RandomOrder, new StandardSQLFunction("NEWID", NHibernateUtil.Guid));
            //RegisterFunction("NEWID()", new StandardSQLFunction("NEWID()", NHibernateUtil.Guid));

            CustomProjections.Register();
        }


    }

    [Serializable]
    public class MySqliteDialect : SQLiteDialect
    {
        protected override void RegisterFunctions()
        {
            base.RegisterFunctions();
            RegisterFunction(SbDialect.RandomOrder, new StandardSQLFunction("random", NHibernateUtil.Guid));
        }
    }
    public static class DialectExtensions
    {
        public static bool FullTextContains(this string source, string pattern)
        {
            return false;
        }
        public static bool IsLike2(this string source, string pattern)
        {
            return false;
        }
    }

    public sealed class MyLinqToHqlGeneratorsRegistry : DefaultLinqToHqlGeneratorsRegistry
    {
        public MyLinqToHqlGeneratorsRegistry()
        {
            RegisterGenerator(NHibernate.Util.ReflectHelper.GetMethod(() => DialectExtensions.FullTextContains(null, null)),
                new FullTextContainsGenerator());

            RegisterGenerator(NHibernate.Util.ReflectHelper.GetMethod(() => DialectExtensions.IsLike2(null, null)),
                new IsLikeGenerator());
        }
    }

    public class IsLikeGenerator : BaseHqlGeneratorForMethod
    {
        public IsLikeGenerator()
        {
            SupportedMethods = new[] {NHibernate.Util.ReflectHelper.GetMethod(() => DialectExtensions.IsLike2(null, null))};
        }

        public override HqlTreeNode BuildHql(MethodInfo method,
            System.Linq.Expressions.Expression targetObject,
            ReadOnlyCollection<System.Linq.Expressions.Expression> arguments,
            HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
        
        {
            var args = new[] {
                visitor.Visit(arguments[0]).AsExpression(),
                visitor.Visit(arguments[1]).AsExpression()
            };
           // treeBuilder.AddSe
            return treeBuilder.Like(visitor.Visit(arguments[0]).AsExpression(),
                visitor.Visit(arguments[1]).AsExpression());
        }
    }

    public class FullTextContainsGenerator : BaseHqlGeneratorForMethod
    {
        public FullTextContainsGenerator()
        {
            SupportedMethods = new[] { NHibernate.Util.ReflectHelper.GetMethod(() => DialectExtensions.FullTextContains(null, null)) };
        }

        public override HqlTreeNode BuildHql(MethodInfo method,
            System.Linq.Expressions.Expression targetObject,
            ReadOnlyCollection<System.Linq.Expressions.Expression> arguments,
            HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
        {
            var args = new[] {
                visitor.Visit(arguments[0]).AsExpression(),
                visitor.Visit(arguments[1]).AsExpression()
            };
            return treeBuilder.BooleanMethodCall("contains", args);
        }
    }


    //public static class CustomLinqExtensions
    //{
    //    [LinqExtensionMethod("string_agg")]
    //    public static string StringAgg(this string searchField, string seperator)
    //    {
    //        // No need to implement it in .Net, unless you wish to call it
    //        // outside IQueryable context too.
    //        throw new NotImplementedException("This call should be translated " +
    //                                          "to SQL and run db side, but it has been run with .Net runtime");
    //    }
    //}

}