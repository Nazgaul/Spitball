using NHibernate.Dialect.Function;


namespace Zbang.Zbox.Infrastructure.Data.Dialect
{
    public class ZboxDialect : NHibernate.Dialect.MsSql2012Dialect // MsSqlAzure2008Dialect
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
