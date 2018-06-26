using Cloudents.Core.Entities.Db;

namespace Cloudents.Infrastructure.Data.Maps
{
    public class TransactionMap : SpitballClassMap<Transaction>
    {
        public TransactionMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            References(x => x.User).ForeignKey("Transaction_User").Not.Nullable();
            Map(x => x.Created).Not.Nullable();
            Map(x => x.Action).Not.Nullable();
            Map(x => x.Type).Not.Nullable();
            Map(x => x.Price).Not.Nullable().CustomSqlType("smallmoney");
            Map(x => x.Balance).Not.Nullable().CustomSqlType("smallmoney");
           // References(x => x.NextTransaction);
        }
    }
}