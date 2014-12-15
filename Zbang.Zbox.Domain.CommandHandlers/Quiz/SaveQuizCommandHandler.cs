﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers.Quiz
{
    public class SaveQuizCommandHandler : ICommandHandlerAsync<SaveQuizCommand, SaveQuizCommandResult>
    {
        private readonly IRepository<Domain.Quiz> m_QuizRepository;
        private readonly IQueueProvider m_QueueProvider;
        private readonly IBoxRepository m_BoxRepository;
        private readonly IItemRepository m_ItemRepository;
        private readonly IIdGenerator m_IdGenerator;
        private readonly IRepository<Comment> m_CommentRepository;
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<Reputation> m_ReputationRepository

            ;

        public SaveQuizCommandHandler(
            IRepository<Domain.Quiz> quizRepository,
            IQueueProvider queueProvider,
            IBoxRepository boxRepository,
            IItemRepository itemRepository,
            IIdGenerator idGenerator,
            IRepository<Comment> commentRepository, IUserRepository userRepository, IRepository<Reputation> reputationRepository)
        {
            m_QuizRepository = quizRepository;
            m_QueueProvider = queueProvider;
            m_BoxRepository = boxRepository;
            m_ItemRepository = itemRepository;
            m_IdGenerator = idGenerator;
            m_CommentRepository = commentRepository;
            m_UserRepository = userRepository;
            m_ReputationRepository = reputationRepository;
        }
        public async Task<SaveQuizCommandResult> ExecuteAsync(SaveQuizCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");
            var quiz = m_QuizRepository.Load(message.QuizId);
            if (quiz.Owner.Id != message.UserId)
            {
                throw new UnauthorizedAccessException("user is not owner of quiz");
            }

            if (string.IsNullOrEmpty(quiz.Name))
            {
                throw new ArgumentException("quiz name is empty");
            }
            foreach (var question in quiz.Questions)
            {
                if (string.IsNullOrWhiteSpace(question.Text))
                {
                    throw new ArgumentException("question text is empty");
                }
                if (question.RightAnswer == null)
                {
                    throw new ArgumentException("question have no right answer");
                }
                if (question.Answers.Count() < 2)
                {
                    throw new ArgumentException("question answers are below 2");
                }
                if (question.Answers.Any(w => w.Text == null))
                {
                    throw new ArgumentException("question answers don't have text");
                }

            }

            quiz.Publish = true;
            var sb = new StringBuilder();
            foreach (var question in quiz.Questions)
            {
                sb.AppendFormat("{0} ", question.Text);
            }

            quiz.Content = sb.ToString().Substring(0, Math.Min(sb.Length, 254));

            quiz.Box.UserTime.UpdateUserTime(quiz.Owner.Email);
            quiz.Box.UpdateItemCount();
            quiz.GenerateUrl();
            m_BoxRepository.Save(quiz.Box);
            m_QuizRepository.Save(quiz);

            var comment = m_ItemRepository.GetPreviousCommentId(quiz.Box, quiz.Owner) ??
                         new Comment(quiz.Owner, null, quiz.Box, m_IdGenerator.GetId(), null, true);
            comment.AddQuiz(quiz);
            m_CommentRepository.Save(comment);

            m_ReputationRepository.Save(quiz.Owner.AddReputation(ReputationAction.AddQuiz));
            m_UserRepository.Save(quiz.Owner);

            var t1 = m_QueueProvider.InsertMessageToTranactionAsync(new UpdateData(quiz.Owner.Id, quiz.Box.Id, null, null, null, quiz.Id));
            var t2 = m_QueueProvider.InsertMessageToTranactionAsync(new ReputationData(quiz.Owner.Id));

           await Task.WhenAll(t1, t2);

            return new SaveQuizCommandResult(quiz.Url);
        }
    }
}
