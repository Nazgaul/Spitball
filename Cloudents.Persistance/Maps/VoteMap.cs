using Cloudents.Core.Entities;
using FluentNHibernate.Mapping;
using JetBrains.Annotations;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Cloudents.Persistence.Maps
{
    [UsedImplicitly]
    public sealed class VoteMap : ClassMapping<Vote>
    {
        public VoteMap()
        {
            // nhibernate doesn't support multiple unique key on the same column
            Id(x => x.Id, c => c.Generator(Generators.GuidComb));
            //Id(x => x.Id).GeneratedBy.GuidComb();

            ManyToOne(x => x.User, c =>
                {
                    c.Column("UserId");
                    c.NotNullable(true);
                    c.ForeignKey("Votes_User");
                });
            //References(x => x.User)
            //    //.UniqueKey("uq_VotesUserDocument")
            //    //.UniqueKey("uq_VotesUserAnswer")
            //    //.UniqueKey("uq_VotesUserQuestion")
            //    .Column("UserId").Not.Nullable().ForeignKey("Votes_User");

            ManyToOne(x => x.Document, c =>
            {
                c.Column("DocumentId");
                c.NotNullable(false);
                c.ForeignKey("Votes_Document");
            });

            //References(x => x.Document)
            //    //.UniqueKey("uq_VotesUserDocument")
            //    .Column("DocumentId").Nullable().ForeignKey("Votes_Document");

            ManyToOne(x => x.Answer, c =>
            {
                c.Column("AnswerId");
                c.NotNullable(false);
                c.ForeignKey("Votes_Answer");
            });
            //References(x => x.Answer)
            //    //.UniqueKey("uq_VotesUserAnswer")
            //    .Column("AnswerId").Nullable().ForeignKey("Votes_Answer");

            ManyToOne(x => x.Question, c =>
            {
                c.Column("QuestionId");
                c.NotNullable(false);
                c.ForeignKey("Votes_Question");
            });
            //References(x => x.Question)
            //    //.UniqueKey("uq_VotesUserQuestion")
            //    .Column("QuestionId").Nullable().ForeignKey("Votes_Question");
            Property(x => x.VoteType, c => {
                c.Type<NHibernate.Type.EnumType<VoteType>>();
                c.NotNullable(true);
                });
            //Map(x => x.VoteType).CustomType<VoteType>().Not.Nullable();
            Component(x => x.TimeStamp);

            SchemaAction(NHibernate.Mapping.ByCode.SchemaAction.Validate);
        }
    }

    /*
     * ALTER TABLE [sb].[Vote] ADD UNIQUE NONCLUSTERED (
	[UserId] ASC,
	[QuestionId] ASC)
    ALTER TABLE [sb].[Vote] ADD UNIQUE NONCLUSTERED (
	[UserId] ASC,
	[AnswerId] ASC)
    ALTER TABLE [sb].[Vote] ADD UNIQUE NONCLUSTERED (
	[UserId] ASC,
	[DocumentId] ASC)
     */
}