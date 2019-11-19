using NHibernate;
using NHibernate.Dialect;
using NHibernate.Dialect.Function;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using System;
using System.Collections.ObjectModel;
using System.Reflection;

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

            RegisterFunction(RandomOrder, new StandardSQLFunction("NEWID", NHibernateUtil.Guid));
            //RegisterFunction("NEWID()", new StandardSQLFunction("NEWID()", NHibernateUtil.Guid));

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
    }

    public sealed class MyLinqToHqlGeneratorsRegistry : DefaultLinqToHqlGeneratorsRegistry
    {
        public MyLinqToHqlGeneratorsRegistry()
        {
            RegisterGenerator(NHibernate.Util.ReflectHelper.GetMethod(() => DialectExtensions.FullTextContains(null, null)),
                new FullTextContainsGenerator());
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

}