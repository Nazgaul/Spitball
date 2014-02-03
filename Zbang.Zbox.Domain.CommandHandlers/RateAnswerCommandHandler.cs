using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class RateAnswerCommandHandler : ICommandHandler<RateAnswerCommand>
    {
        private readonly IAnswerRatingRepository m_AnswerRatingRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<Answer> m_AnswerRepository;

        public RateAnswerCommandHandler(IAnswerRatingRepository answerRatingRepository,
            IUserRepository userRepository,
            IRepository<Answer> answerRepository)
        {
            m_AnswerRatingRepository = answerRatingRepository;
            m_UserRepository = userRepository;
            m_AnswerRepository = answerRepository;
        }
        public void Handle(RateAnswerCommand message)
        {
            var answerUp = m_AnswerRatingRepository.GetAnswerRating(message.UserId, message.AnswerId);

            if (answerUp == null)
            {
                AddItemLike(message);
            }
            else
            {
                ChangeRating(answerUp);
            }
        }


        private void ChangeRating(AnswerRating answerRating)
        {

            answerRating.ChangeUserRating();
            m_AnswerRatingRepository.Save(answerRating);
        }



        private void AddItemLike(RateAnswerCommand message)
        {
            var user = m_UserRepository.Get(message.UserId);
            var answer = m_AnswerRepository.Get(message.AnswerId);

            var answerRating = new AnswerRating(message.Id, user, answer);
            m_AnswerRatingRepository.Save(answerRating);
        }
    }
}
