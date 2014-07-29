﻿using System;
using System.Linq;
using Zbang.Zbox.Domain.Commands.Quiz;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers.Quiz
{
    public class CreateDiscussionCommandHandler : ICommandHandler<CreateDiscussionCommand>
    {
        // private readonly IIdGenerator m_IdGenerator;
        private readonly IRepository<Question> m_QuestionRepository;
        private readonly IRepository<Domain.Quiz> m_QuizRepository;
        private readonly IRepository<Discussion> m_DiscussionRepository;
        private readonly IUserRepository m_UserRepository;

        public CreateDiscussionCommandHandler(
            IUserRepository userRepository,
            IRepository<Question> questionRepository,
            IRepository<Discussion> discussionRepository,
            IRepository<Domain.Quiz> quizRepository

            )
        {
            m_QuestionRepository = questionRepository;
            m_DiscussionRepository = discussionRepository;
            m_UserRepository = userRepository;
            m_QuizRepository = quizRepository;
        }
        public void Handle(CreateDiscussionCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");

            if (string.IsNullOrEmpty(message.Text))
            {
                throw new NullReferenceException("message.Text");
            }

            var user = m_UserRepository.Load(message.UserId);
            var question = m_QuestionRepository.Load(message.QuestionId);
            

            var discussion = new Discussion(message.DiscussionId, user, TextManipulation.EncodeText(message.Text), question);

            var noOfDiscussion = m_DiscussionRepository.GetQuerable().Count(w => w.Quiz == question.Quiz);
            question.Quiz.UpdateNumberOfComments(noOfDiscussion + 1); // the current one is not saved yet


            m_QuizRepository.Save(question.Quiz);
            m_DiscussionRepository.Save(discussion);
        }
    }
}
