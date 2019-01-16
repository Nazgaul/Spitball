using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistance.Maps
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "assembly loader inject")]
    public class TransactionMap : ClassMap<Transaction>
    {
        public TransactionMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            References(x => x.User).ForeignKey("Transaction_User").Not.Nullable();
            Map(x => x.Created).Not.Nullable();
            Component(x => x.TransactionType, y =>
            {
                y.Map(z => z.Action).Not.Nullable();
                y.Map(z => z.Type).Not.Nullable();
                y.Map(z => z.Price).Not.Nullable().CustomSqlType("smallmoney");
            });
            

            References(x => x.Question).Column("QuestionId").ForeignKey("Transaction_Question").Nullable();
            References(x => x.Answer).Column("AnswerId").ForeignKey("Transaction_Answer").Nullable();
            References(x => x.InvitedUser).Column("InvitedUserId").ForeignKey("Transaction_InvitedUser").Nullable();
            References(x => x.Document).Column("DocumentId").ForeignKey("Transaction_Document").Nullable();
            SchemaAction.None();
        }
    }
}