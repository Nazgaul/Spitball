using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class MarkAsAnswerCommandHandler : ICommandHandler<MarkAsAnswerCommand>
    {
        private readonly IQuestionRepository m_QuestionRepository;
        private readonly IRepository<CommentReplies> m_AnswerRepository;
        private readonly IUserRepository m_UserRepository;

        public MarkAsAnswerCommandHandler(IQuestionRepository questionRepository, IRepository<CommentReplies> answerRepository, IUserRepository userRepository)
        {
            m_QuestionRepository = questionRepository;
            m_AnswerRepository = answerRepository;
            m_UserRepository = userRepository;
        }
        public void Handle(MarkAsAnswerCommand message)
        {
            Throw.OnNull(message, "message");

            var markedAnswer = m_AnswerRepository.Load(message.AnswerId);

            var userType = m_UserRepository.GetUserToBoxRelationShipType(message.UserId, markedAnswer.Box.Id); //user.GetUserType(box.Id);
            if (userType == UserRelationshipType.None || userType == UserRelationshipType.Invite)
            {
                throw new UnauthorizedAccessException("User is not connected to box");
            }
            if (markedAnswer.Question.User.Id != message.UserId)
            {
                throw new UnauthorizedAccessException("User is not ask the question");
            }
            var oldMarkedAnswer = m_QuestionRepository.GetAnswers(markedAnswer.Question).Where(w => w.MarkAnswer == true).FirstOrDefault();
            if (oldMarkedAnswer != null)
            {
                oldMarkedAnswer.MarkAnswer = false;
                m_AnswerRepository.Save(oldMarkedAnswer);
            }
            markedAnswer.MarkAnswer = true;
            m_AnswerRepository.Save(markedAnswer);



        }
    }
}
