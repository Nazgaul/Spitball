using System;
using NHibernate;
using NHibernate.Dialect;
using NHibernate.Dialect.Function;

namespace Cloudents.Persistence
{
    [Serializable]
    public class SbDialect : MsSql2012Dialect
    {
        internal const string RandomOrder = "random_Order";

        protected override void RegisterFunctions()
        {
            base.RegisterFunctions();
            RegisterFunction("FullTextContains", new StandardSQLFunction("contains", null));

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

    //public class MyLinqToHqlGeneratorsRegistry : DefaultLinqToHqlGeneratorsRegistry
    //{
    //    public MyLinqToHqlGeneratorsRegistry()
    //        : base()
    //    {

    //        CalculatedPropertyGenerator<User, decimal>.Register(this, x => x.Fee, User.CalculateBalanceExpression);
    //    }
    //}

    //public class CalculatedPropertyGenerator<T, TResult> : BaseHqlGeneratorForProperty
    //{
    //    public static void Register(ILinqToHqlGeneratorsRegistry registry, Expression<Func<T, TResult>> property, Expression<Func<T, TResult>> calculationExp)
    //    {
    //        registry.RegisterGenerator(NHibernate.Util.ReflectHelper.GetProperty(property), new CalculatedPropertyGenerator<T, TResult> { _calculationExp = calculationExp });
    //    }

    //    private CalculatedPropertyGenerator() { }

    //    private Expression<Func<T, TResult>> _calculationExp;

    //    public override HqlTreeNode BuildHql(MemberInfo member, Expression expression, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
    //    {
    //        return visitor.Visit(_calculationExp);
    //    }
    //}

    //public class SqlFunctions
    //{
    //    [LinqExtensionMethod("NEWID")] public static Guid NewID() { return Guid.NewGuid(); }
    //}
}