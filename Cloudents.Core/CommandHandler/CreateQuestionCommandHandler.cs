using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using JetBrains.Annotations;

namespace Cloudents.Core.CommandHandler
{
    [UsedImplicitly]
    public class CreateQuestionCommandHandler : ICommandHandler<CreateQuestionCommand>
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IBlobProvider<QuestionAnswerContainer> _blobProvider;
        private readonly ITextAnalysis _textAnalysis;

        public CreateQuestionCommandHandler(IQuestionRepository questionRepository,
             IRepository<User> userRepository, ITextAnalysis textAnalysis, IBlobProvider<QuestionAnswerContainer> blobProvider = null)
        {
            _questionRepository = questionRepository;
            _userRepository = userRepository;
            _textAnalysis = textAnalysis;
            _blobProvider = blobProvider;
        }

        public async Task ExecuteAsync(CreateQuestionCommand message, CancellationToken token)
        {
            //if you get an exception doing debug make sure the locals window is minimized.
            var user = await _userRepository.LoadAsync(message.UserId, token).ConfigureAwait(true);

            var oldQuestion = await _questionRepository.GetUserLastQuestionAsync(user.Id, token);

            if (oldQuestion?.Created.AddSeconds(20) > DateTime.UtcNow)
            {
                throw new InvalidOperationException("You need to wait before asking more questions");
            }

            var textLanguage = await _textAnalysis.DetectLanguageAsync(message.Text, token);

            var question = new Question(message.SubjectId,
                message.Text, message.Price, message.Files?.Count() ?? 0, user, message.Color);
            question.SetLanguage(textLanguage);
            await _questionRepository.AddAsync(question, token).ConfigureAwait(true);
            var id = question.Id;

            if (_blobProvider != null)
            {
                var l = message.Files?.Select(file => _blobProvider.MoveAsync(file, $"question/{id}", token)) ??
                        Enumerable.Empty<Task>();
                await Task.WhenAll(l).ConfigureAwait(true);
            }
            

            message.Id = id;
        }
    }
}
