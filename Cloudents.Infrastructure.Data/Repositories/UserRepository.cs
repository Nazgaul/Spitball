using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Data.Repositories
{
    [UsedImplicitly]
    public class UserRepository : NHibernateRepository<User>, IUserRepository
    {
        public UserRepository(ISession session) : base(session)
        {
        }

        //public UserRepository(IIndex<Core.Enum.Database, IUnitOfWork> unitOfWork) : base(unitOfWork)
        //{
        //}

        public Task<User> GetUserByExpressionAsync(Expression<Func<User, bool>> expression, CancellationToken token)
        {
            return Session.Query<User>().FirstOrDefaultAsync(expression, token);
        }

        public async Task<UserAccountDto> GetUserDetailAsync(long id, CancellationToken token)
        {
            var p = await  Session.Query<User>().Fetch(f=>f.LastTransaction).Where(w => w.Id == id).Select(s => new UserAccountDto()
            {
                Id = s.Id,
                 Balance = s.LastTransaction.Balance,
                Name = s.Name,
                Image = s.Image
            }).SingleOrDefaultAsync();//.ToFutureValue();

            //var balance = await Session.Query<Transaction>().Where(w => w.User.Id == id && w.NextTransaction == null).Select(s => s.Balance)
            //    .SingleOrDefaultAsync(token);

            //var user = p.Value;

            //user.Balance = balance;
            return p;

        }

        public async Task<IList<User>> GetAllUsersAsync(CancellationToken token)
        {
            var t = await Session.Query<User>().Where(w => w.Email != null).ToListAsync(token);
            return t;
        }

        public async Task<ProfileDto> GetUserProfileAsync(long id, CancellationToken token)
        {
            var futureDto = Session.Query<User>()
                .Fetch(u => u.University)
                .Where(w => w.Id == id)
                .Select(s => new ProfileDto
                {
                    User = new UserProfileDto
                    {
                        Id = s.Id,
                        Image = s.Image,
                        Name = s.Name,
                        UniversityName = s.University.Name
                    }
                })
                .ToFutureValue();
            var futureQuestions = Session.Query<Question>()
                .Fetch(f => f.Subject)
                .Where(w => w.User.Id == id)
                .Select(s => new QuestionDto
                {
                    Text = s.Text,
                    Answers = s.Answers.Count(),
                    DateTime = s.Created,
                    Files = s.Attachments,
                    Price = s.Price,
                    Id = s.Id,
                    Subject = s.Subject.Text
                }).ToFuture();

            var futureAnswers = Session.Query<Answer>()
                .Fetch(f => f.Question).ThenFetch(f => f.Subject)
                .Where(w => w.User.Id == id)
                .Select(s => new QuestionDto
                {
                    Text = s.Question.Text,
                    Answers = s.Question.Answers.Count(),
                    DateTime = s.Question.Created,
                    Files = s.Question.Attachments,
                    Price = s.Question.Price,
                    Id = s.Question.Id,
                    Subject = s.Question.Subject.Text
                }).ToFuture();

            //var futureQuestions = Session.Query<Question>()
            //    .Fetch(f => f.Subject)
            //    .Where(w => w.User.Id == id)
            //    .Select(s => new
            //    {
            //        s.CorrectAnswer,
            //        s.Id,
            //        SubjectText = s.Subject.Text,
            //        s.Text,
            //        s.Price
            //    })
            //    .ToFuture();

            var dto = await futureDto.GetValueAsync(token).ConfigureAwait(false);
            if (dto == null)
            {
                return null;
            }
            //var questions = await futureQuestions.GetEnumerableAsync(token).ConfigureAwait(false);

            //var questionsLookup = questions.ToLookup(t => t.CorrectAnswer == null);
            dto.Ask = await futureQuestions.GetEnumerableAsync(token);
            dto.Answer = await futureAnswers.GetEnumerableAsync(token);
            //dto.Ask = questionsLookup[true].Select(s => new QuestionDto
            //{
            //    Id = s.Id,
            //    Subject = s.SubjectText,
            //    Text = s.Text,
            //    Price = s.Price
            //});
            //dto.Answer = questionsLookup[false].Select(s => new QuestionDto
            //{
            //    Id = s.Id,
            //    Subject = s.SubjectText,
            //    Text = s.Text,
            //    Price = s.Price
            //});

            return dto;
        }
    }
}