using System;
using System.Linq;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers.Quiz
{
    class DeleteQuizCommandHandler : ICommandHandler<DeleteQuizCommand>
    {
        private readonly IRepository<Domain.Quiz> m_QuizRepository;
        private readonly IRepository<Box> m_BoxRepository;
        private readonly IRepository<Comment> m_CommentRepository;
       
        private readonly IUserRepository m_UserRepository;
        private readonly IQueueProvider m_QueueProvider;

        public DeleteQuizCommandHandler(IRepository<Domain.Quiz> quizRepository,
            IUserRepository userRepository, IRepository<Box> boxRepository, IQueueProvider queueProvider, IRepository<Comment> commentRepository)
        {
            m_QuizRepository = quizRepository;
          
            m_UserRepository = userRepository;
            m_BoxRepository = boxRepository;
            m_QueueProvider = queueProvider;
            m_CommentRepository = commentRepository;
        }
        public void Handle(DeleteQuizCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");

            var quiz = m_QuizRepository.Load(message.QuizId); // need that because we save box
            var userType = m_UserRepository.GetUserToBoxRelationShipType(message.UserId, quiz.Box.Id);

            bool isAuthorize = userType == UserRelationshipType.Owner
               || Equals(quiz.Owner.Id, message.UserId)
               || quiz.Owner.IsAdmin();

            if (!isAuthorize)
            {
                throw new UnauthorizedAccessException("User is unauthorized to delete file");
            }

            if (quiz.Comment != null)
            {
                var needRemove = quiz.Comment.RemoveQuiz(quiz);
                if (needRemove)
                {
                    m_CommentRepository.Delete(quiz.Comment);
                }
                else
                {
                    m_CommentRepository.Save(quiz.Comment);
                }
            }
            m_QueueProvider.InsertMessageToTranaction(new ReputationData(quiz.Owner.Id));
            m_QuizRepository.Delete(quiz, true);
            quiz.Box.UpdateItemCount();
            m_BoxRepository.Save(quiz.Box);
        }
    }
}
