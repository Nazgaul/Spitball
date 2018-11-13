using System;
using Cloudents.Core.Attributes;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.CommandHandler.Admin
{
    [AdminCommandHandler]
    public class CreateQuestionCommandHandler : ICommandHandler<CreateQuestionCommand>
    {

        private readonly IUserRepository _userRepository;
        private readonly IRepository<Question> _questionRepository;
        private readonly ITextAnalysis _textAnalysis;


        public CreateQuestionCommandHandler(IUserRepository userRepository, IRepository<Question> questionRepository, ITextAnalysis textAnalysis)
        {
            _userRepository = userRepository;
            _questionRepository = questionRepository;
            _textAnalysis = textAnalysis;
        }


        public async Task ExecuteAsync(CreateQuestionCommand message, CancellationToken token)
        {
            var user = await _userRepository.GetRandomFictiveUserAsync(message.Country, token);
            if (user == null)
            {
                throw new InvalidOperationException("we don't have fictive user in that country");
            }
            var textLanguage = await _textAnalysis.DetectLanguageAsync(message.Text, token);
            var question = new Question(message.SubjectId, message.Text, message.Price, 0, user,
                QuestionColor.Default, textLanguage)
            {
                State = QuestionState.Pending
            };
            await _questionRepository.AddAsync(question, token).ConfigureAwait(true);

        }
    }
}