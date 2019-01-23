using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Attributes;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Dapper;

namespace Cloudents.Query.Email
{
    public class GetAnswerAcceptedEmailQuery :IQuery<AnswerAcceptedEmailDto>
    {
        public GetAnswerAcceptedEmailQuery(Guid transactionId)
        {
            TransactionId = transactionId;
        }

        private Guid TransactionId { get; }


        internal sealed class GetAnswerAcceptedEmailQueryQueryHandler : IQueryHandler<GetAnswerAcceptedEmailQuery, AnswerAcceptedEmailDto>
        {
            private readonly IConfigurationKeys _provider;

            public GetAnswerAcceptedEmailQueryQueryHandler(IConfigurationKeys provider)
            {
                _provider = provider;
            }

            public async Task<AnswerAcceptedEmailDto> GetAsync(GetAnswerAcceptedEmailQuery query, CancellationToken token)
            {
                const string sql = @"Select e.language, e.SocialShare,e.Subject,
 ek.Title,
 ek.Subtitle,
 ek.Body,
 ek.Cta,
 ek.minorTitle,
 d.CourseName,
  d.Name as documentName ,
u.Email,
 u.id as userId,
 t.Price as tokens
  from  sb.[Transaction] t
join sb.[User] u on t.User_id = u.Id
join sb.Email e on u.Language = e.Language and e.Event=@e
join sb.emailBlock ek on e.Id = ek.email_id
join sb.Document d on t.DocumentId = d.id
where t.id = @id
order by ek.orderBlock ";
                using (var connection = new SqlConnection(_provider.Db.Db))
                {
                    var result = await connection.QuerySingleOrDefaultAsync<DbClass>(sql,
                        new
                        {
                            id = query.TransactionId,
                            e = (string)SystemEvent.AnswerAccepted
                        });


                    var retVal = new AnswerAcceptedEmailDto()
                    {
                        Language = result.Language,
                        AnswerText = result.AnswerText,
                        QuestionText = result.QuestionText,
                        SocialShare = result.SocialShare,
                        Subject = result.Subject,
                        ToEmailAddress = result.Email,
                        UserId = result.UserId,
                        Tokens = result.Tokens


                    };
                    var emailBlock = new EmailBlockDto()
                    {
                        Title = result.Title,
                        Body = result.Body,
                        Subtitle = result.Subtitle,
                        Cta = result.Cta,
                        MinorTitle = result.MinorTitle
                    };
                    retVal.Blocks.Add(emailBlock);
                    return retVal;
                }
            }
        }




        private class DbClass
        {
            [DtoToEntityConnection(nameof(Core.Entities.Email.Language))]
            public string Language { get; private set; }
            [DtoToEntityConnection(nameof(Core.Entities.Email.SocialShare))]
            public bool SocialShare { get; set; }
            [DtoToEntityConnection(nameof(Core.Entities.Email.Subject))]
            public string Subject { get; set; }
            [DtoToEntityConnection(nameof(EmailBlock.Title))]
            public string Title { get; set; }
            [DtoToEntityConnection(nameof(EmailBlock.SubTitle))]
            public string Subtitle { get; set; }
            [DtoToEntityConnection(nameof(EmailBlock.Body))]
            public string Body { get; set; }
            [DtoToEntityConnection(nameof(EmailBlock.Cta))]
            public string Cta { get; set; }
            [DtoToEntityConnection(nameof(EmailBlock.MinorTitle))]
            public string MinorTitle { get; set; }
            [DtoToEntityConnection(nameof(RegularUser.Email))]
            public string Email { get; set; }
            [DtoToEntityConnection(nameof(RegularUser.Id))]
            public int UserId { get; set; }
            [DtoToEntityConnection(nameof(Transaction.TransactionType.Price))]
            public decimal Tokens { get; set; }
            [DtoToEntityConnection(nameof(Question.Text))]
            public string QuestionText { get; set; }
            [DtoToEntityConnection(nameof(Answer.Text))]
            public string AnswerText { get; set; }
        }
    }
}