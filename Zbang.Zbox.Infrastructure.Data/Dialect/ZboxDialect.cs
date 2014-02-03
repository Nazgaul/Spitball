using NHibernate;
using NHibernate.Dialect;
using NHibernate.Dialect.Function;
using NHibernate.SqlCommand;


namespace Zbang.Zbox.Infrastructure.Data.Dialect
{
    public class ZboxDialect :  MsSqlAzure2008Dialect
    {
        protected override void RegisterFunctions()
        {
            base.RegisterFunctions();
            //RegisterFunction("GetThumbnailUrl", new NoArgSQLFunction("Zbox.GetThumbnailUrl", NHibernateUtil.String, true));
        }

        //get from MsSqlAzure2008Dialect
        public override string PrimaryKeyString
        {
            get { return "primary key CLUSTERED"; }
        }

        //public override SqlString GetLimitString(SqlString querySqlString, SqlString offset, SqlString limit)
        //{
            
        //    var tokenEnum = new NHibernate.SqlCommand.Parser.SqlTokenizer(querySqlString).GetEnumerator();
        //    if (!tokenEnum.TryParseUntilFirstMsSqlSelectColumn()) return null;

        //    var result = new SqlStringBuilder(querySqlString);
        //    if (!tokenEnum.TryParseUntil("order"))
        //    {
        //        result.Add(" ORDER BY CURRENT_TIMESTAMP");
        //    }

        //    result.Add(" OFFSET ");
        //    if (offset != null)
        //    {
        //        result.Add(offset).Add(" ROWS");
        //    }
        //    else
        //    {
        //        result.Add("0 ROWS");
        //    }

        //    if (limit != null)
        //    {
        //        result.Add(" FETCH FIRST ").Add(limit).Add(" ROWS ONLY");
        //    }

        //    return result.ToSqlString();
        //}
    }

    
}
