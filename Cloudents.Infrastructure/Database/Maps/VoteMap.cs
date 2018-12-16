﻿using Cloudents.Domain.Entities;
using FluentNHibernate.Mapping;

namespace Cloudents.Infrastructure.Database.Maps
{
    public class VoteMap : ClassMap<Vote>
    {
        public VoteMap()
        {
            // nhibernate doesn't support multiple unique key on the same column
            Id(x => x.Id).GeneratedBy.Guid();
            References(x => x.User)
                //.UniqueKey("uq_VotesUserDocument")
                //.UniqueKey("uq_VotesUserAnswer")
                //.UniqueKey("uq_VotesUserQuestion")
                .Column("UserId").Not.Nullable().ForeignKey("Votes_User");
            References(x => x.Document)
                //.UniqueKey("uq_VotesUserDocument")
                .Column("DocumentId").Nullable().ForeignKey("Votes_Document");
            References(x => x.Answer)
                //.UniqueKey("uq_VotesUserAnswer")
                .Column("AnswerId").Nullable().ForeignKey("Votes_Answer");
            References(x => x.Question)
                //.UniqueKey("uq_VotesUserQuestion")
                .Column("QuestionId").Nullable().ForeignKey("Votes_Question");
            Map(x => x.VoteType).CustomType<int>().Not.Nullable();
            Component(x => x.TimeStamp);

            SchemaAction.None();
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