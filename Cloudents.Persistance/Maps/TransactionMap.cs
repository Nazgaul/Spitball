using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Entities;
//using FluentNHibernate.Mapping;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Mapping.ByCode;

namespace Cloudents.Persistence.Maps
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "assembly loader inject")]
    public class TransactionMap : ClassMapping<Transaction>
    {
        public TransactionMap()
        {
            Id(x => x.Id, c => {
                c.Column("Id");
                c.Generator(Generators.GuidComb);
                });
            //Id(x => x.Id).GeneratedBy.GuidComb();
            ManyToOne(x => x.User, c => {
                c.Column("User_id");
                c.ForeignKey("Transaction_User");
                c.NotNullable(true);
            });
            //References(x => x.User)
            //    .Column("User_id").ForeignKey("Transaction_User").Not.Nullable();
            Property(x => x.Created, c => c.NotNullable(true));
            //Map(x => x.Created).Not.Nullable();
            Component(x => x.Action);
            //Map(z => z.Action).Not.Nullable();
            Component(x => x.Type);
            //Map(z => z.Type).Not.Nullable();
            Property(x => x.Price, c => {
                c.NotNullable(true);
                c.Column(cl => cl.SqlType("smallmoney"));
            });
            //Map(z => z.Price).Not.Nullable().CustomSqlType("smallmoney");
            Discriminator(d => d.Column("TransactionType"));
            //DiscriminateSubClassesOnColumn("TransactionType");
            //SchemaAction.Validate();
        }
    }

    public class CashOutTransactionMap : SubclassMapping<CashOutTransaction>
    {
        public CashOutTransactionMap()
        {
            DiscriminatorValue("CashOut");
            //DiscriminatorValue("CashOut");
            Property(x => x.Approved, c => {
                c.Column("Approved");
                c.NotNullable(false);
            });
            //Map(x => x.Approved).Column("Approved").Nullable();
            Property(x => x.DeclinedReason, c => {
                c.Column("DeclinedReason");
                c.NotNullable(false);
            });
            //Map(x => x.DeclinedReason).Column("DeclinedReason").Nullable();
        }
    }

    public class BuyPointsTransactionMap : SubclassMapping<BuyPointsTransaction>
    {
        public BuyPointsTransactionMap()
        {
            DiscriminatorValue("BuyPoints");
            Property(x => x.TransactionId, c => {
                c.Column("PayPalTransactionId");
                c.NotNullable(true);
            });
            //Map(x => x.TransactionId).Column("PayPalTransactionId").Not.Nullable();
        }
    }

    public class AwardMoneyTransactionMap : SubclassMapping<AwardMoneyTransaction>
    {
        public AwardMoneyTransactionMap()
        {
            DiscriminatorValue("Award");
        }
    }

    public class CommissionTransactionMap : SubclassMapping<CommissionTransaction>
    {
        public CommissionTransactionMap()
        {
            DiscriminatorValue("Commission");
        }
    }


    public class ReferUserTransactionMap : SubclassMapping<ReferUserTransaction>
    {
        public ReferUserTransactionMap()
        {
            DiscriminatorValue("Refer");
            ManyToOne(x => x.InvitedUser, c => {
                c.Column("InvitedUserId");
                c.ForeignKey("Transaction_InvitedUser");
                c.NotNullable(false);
            });
            //References(x => x.InvitedUser).Column("InvitedUserId").ForeignKey("Transaction_InvitedUser").Nullable();

        }
    }

    public class QuestionTransactionMap : SubclassMapping<QuestionTransaction>
    {
        public QuestionTransactionMap()
        {
            DiscriminatorValue("Question");
            ManyToOne(x => x.Question, c => {
                c.Column("QuestionId");
                c.ForeignKey("Transaction_Question");
                c.NotNullable(false);
            });
            //References(x => x.Question)
            //    .Column("QuestionId")
            //    .ForeignKey("Transaction_Question").Nullable();
            ManyToOne(x => x.Answer, c => {
                c.Column("AnswerId");
                c.ForeignKey("Transaction_Answer");
                c.NotNullable(false);
            });
            //References(x => x.Answer).Column("AnswerId").ForeignKey("Transaction_Answer").Nullable();

        }
    }

    public class DocumentTransactionMap : SubclassMapping<DocumentTransaction>
    {
        public DocumentTransactionMap()
        {
            DiscriminatorValue("Document");
            ManyToOne(x => x.Document, c => {
                c.Column("DocumentId");
                c.ForeignKey("Transaction_Document");
                c.NotNullable(false);
            });
            //References(x => x.Document).Column("DocumentId").ForeignKey("Transaction_Document").Nullable();
        }
    }
}