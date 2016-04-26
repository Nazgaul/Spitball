using NHibernate.Dialect;
using NHibernate.Dialect.Function;


namespace Zbang.Zbox.Infrastructure.Data.Dialect
{
    public class ZboxDialect : MsSqlAzure2008Dialect
    {
        protected override void RegisterFunctions()
        {
            base.RegisterFunctions();
            RegisterFunction("CHARINDEX", new StandardSQLFunction("CHARINDEX"));
        }

        //get from MsSqlAzure2008Dialect
        public override string PrimaryKeyString => "primary key CLUSTERED";
    }
}
