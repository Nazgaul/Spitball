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
    public class DeleteQuestionCommandHandler : ICommandHandler<DeleteQuestionCommand>
    {
        private readonly IRepository<Question> m_QuestionRepository;
        private readonly IBoxRepository m_BoxRepository;


        public DeleteQuestionCommandHandler(IRepository<Question> questionRepository, IBoxRepository boxRepository)
        {
            m_QuestionRepository = questionRepository;
            m_BoxRepository = boxRepository;
        }
        public void Handle(DeleteQuestionCommand message)
        {
            var question = m_QuestionRepository.Load(message.QuestionId);
            if (question.User.Id != message.UserId)
            {
                throw new UnauthorizedAccessException("User didnt ask the question");
            }
            var box = question.Box;

            var substract = question.AnswersReadOnly.Count + 1;

            box.UpdateQnACount(m_BoxRepository.QnACount(box.Id) - substract);
            m_BoxRepository.Save(box);

            m_QuestionRepository.Delete(question);

        }
    }
}
