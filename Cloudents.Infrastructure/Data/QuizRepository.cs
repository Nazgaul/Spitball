using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Dapper;

namespace Cloudents.Infrastructure.Data
{
    public class QuizRepository : IReadRepositoryAsync<QuizDto, long>
    {
        private readonly DapperRepository _repository;

        public QuizRepository(DapperRepository repository)
        {
            _repository = repository;
        }

        public Task<QuizDto> GetAsync(long query, CancellationToken token)
        {
            const string quizQuery =
           @"select q.Id, q.Name, u.UserName as Owner, 
q.CreationTime as Date, q.NumberOfViews, q.Publish
from zbox.quiz q 
join zbox.Users u on q.UserId = u.UserId
 where q.id = @QuizId
 and q.isDeleted = 0
and publish = 1;";
            const string questionSql = @"select q.Id, q.Text,q.RightAnswerId as correctAnswer from zbox.QuizQuestion q where QuizId = @QuizId;";

            const string answerSql = @"select a.id, a.text,a.QuestionId from zbox.QuizAnswer a where QuizId = @QuizId;";

            //const string UserQuiz = @"select q.TimeTaken,q.Score from zbox.SolvedQuiz q where q.QuizId = @QuizId and q.UserId = @UserId;";

            //const string UserAnswer = @"select q.AnswerId,q.QuestionId from zbox.SolvedQuestion q where QuizId = @QuizId and UserId = @UserId;";

            const string sql = quizQuery + questionSql + answerSql;
            return _repository.WithConnectionAsync(async c =>
            {
                using (var grid = await c.QueryMultipleAsync(sql, new { QuizId = query }).ConfigureAwait(false))
                {
                    var retVal = new QuizDto
                    {
                        Quiz = await grid.ReadFirstAsync<QuizWithDetailDto>().ConfigureAwait(false)
                    };
                    retVal.Quiz.Questions = await grid.ReadAsync<QuizQuestionWithDetailDto>().ConfigureAwait(false);
                    var answers = (await grid.ReadAsync<QuizAnswerWithDetailDto>().ConfigureAwait(false)).ToLookup(v => v.QuestionId);

                    foreach (var question in retVal.Quiz.Questions)
                    {
                        question.Answers.AddRange(answers[question.Id]);
                    }
                    return retVal;
                }
            }, token);
        }
    }
}
