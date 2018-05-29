﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Database.Repositories
{
    public class UserRepository : NHibernateRepository<User>, IUserRepository
    {
        public UserRepository(UnitOfWork.Factory unitOfWork) : base(unitOfWork)
        {
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
                .Select(s => new
                {
                    s.CorrectAnswer,
                    s.Id,
                    SubjectText = s.Subject.Text,
                    s.Text,
                    s.Price
                    
                })
                .ToFuture();

            var dto = await futureDto.GetValueAsync(token).ConfigureAwait(false);
            var questions = await futureQuestions.GetEnumerableAsync(token).ConfigureAwait(false);

            var questionsLookup = questions.ToLookup(t => t.CorrectAnswer == null);

            dto.Ask = questionsLookup[true].Select(s => new QuestionDto
            {
                Id = s.Id,
                Subject = s.SubjectText,
                Text = s.Text,
                Price = s.Price
            });
            dto.Answer = questionsLookup[false].Select(s => new QuestionDto
            {
                Id = s.Id,
                Subject = s.SubjectText,
                Text = s.Text,
                Price = s.Price
            });

            return dto;
        }
    }
}