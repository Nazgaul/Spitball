﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;

namespace Cloudents.Core.CommandHandler
{
    public class CreateAnswerCommandHandler : ICommandHandlerAsync<CreateAnswerCommand>
    {
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<Answer> _answerRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IBlobProvider<QuestionAnswerContainer> _blobProvider;

        public CreateAnswerCommandHandler(IRepository<Question> questionRepository, IRepository<Answer> answerRepository, IRepository<User> userRepository, IBlobProvider<QuestionAnswerContainer> blobProvider)
        {
            _questionRepository = questionRepository;
            _answerRepository = answerRepository;
            _userRepository = userRepository;
            _blobProvider = blobProvider;
        }

        public async Task HandleAsync(CreateAnswerCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token).ConfigureAwait(false);
            var question = await _questionRepository.LoadAsync(message.QuestionId, token).ConfigureAwait(false);

            var answer = new Answer(question, message.Text, message.Files?.Count() ?? 0, user);
            await _answerRepository.SaveAsync(answer, token).ConfigureAwait(false);
        }
    }
}