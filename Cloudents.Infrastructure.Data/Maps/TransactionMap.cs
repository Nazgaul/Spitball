using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities.Db;

namespace Cloudents.Infrastructure.Data.Maps
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "assmbly loader inject")]
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

            References(x => x.Question).Column("QuestionId").ForeignKey("Transaction_Question").Nullable();
            References(x => x.Answer).Column("AnswerId").ForeignKey("Transaction_Answer").Nullable();
        }
    }
}