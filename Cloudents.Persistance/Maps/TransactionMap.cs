using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Persistence.Maps
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "assembly loader inject")]
    public class TransactionMap : ClassMap<Transaction>
    {
        public TransactionMap()
        {
            Id(x => x.Id).GeneratedBy.GuidComb();
            References(x => x.User)
                .Column("User_id").ForeignKey("Transaction_User").Not.Nullable();
            Map(x => x.Created).Not.Nullable();

            Map(z => z.Action).Not.Nullable();
            Map(z => z.Type).Not.Nullable();
            Map(z => z.Price).Not.Nullable().CustomSqlType("smallmoney");

            DiscriminateSubClassesOnColumn("TransactionType");
            SchemaAction.Validate();
        }
    }

    public class CashOutTransactionMap : SubclassMap<CashOutTransaction>
    {
        public CashOutTransactionMap()
        {
            DiscriminatorValue("CashOut");
            Map(x => x.Approved).Column("Approved").Nullable();
            Map(x => x.DeclinedReason).Column("DeclinedReason").Nullable();
        }
    }

    public class BuyPointsTransactionMap : SubclassMap<BuyPointsTransaction>
    {
        public BuyPointsTransactionMap()
        {
            DiscriminatorValue("BuyPoints");
            Map(x => x.TransactionId).Column("PayPalTransactionId").Not.Nullable();
        }
    }

    public class AwardMoneyTransactionMap : SubclassMap<AwardMoneyTransaction>
    {
        public AwardMoneyTransactionMap()
        {
            DiscriminatorValue("Award");
        }
    }

    public class CommissionTransactionMap : SubclassMap<CommissionTransaction>
    {
        public CommissionTransactionMap()
        {
            DiscriminatorValue("Commission");
        }
    }


    public class ReferUserTransactionMap : SubclassMap<ReferUserTransaction>
    {
        public ReferUserTransactionMap()
        {
            DiscriminatorValue("Refer");
            References(x => x.InvitedUser).Column("InvitedUserId").ForeignKey("Transaction_InvitedUser").Nullable();

        }
    }

    public class QuestionTransactionMap : SubclassMap<QuestionTransaction>
    {
        public QuestionTransactionMap()
        {
            DiscriminatorValue("Question");
            References(x => x.Question)
                .Column("QuestionId")
                .ForeignKey("Transaction_Question").Nullable();
            References(x => x.Answer).Column("AnswerId").ForeignKey("Transaction_Answer").Nullable();

        }
    }

    public class DocumentTransactionMap : SubclassMap<DocumentTransaction>
    {
        public DocumentTransactionMap()
        {
            DiscriminatorValue("Document");
            References(x => x.Document).Column("DocumentId").ForeignKey("Transaction_Document").Nullable();
        }
    }
}