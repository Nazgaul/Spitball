using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class CreateQuestionCommandHandler : ICommandHandler<CreateQuestionCommand>
    {

        private readonly IFictiveUserRepository _userRepository;
        private readonly IRepository<Question> _questionRepository;
        private readonly IUniversityRepository _universityRepository;
        private readonly ITextAnalysis _textAnalysis;
        private readonly IBlobProvider<QuestionAnswerContainer> _blobProvider;
        private readonly ICourseRepository _courseRepository;


        public CreateQuestionCommandHandler(IFictiveUserRepository userRepository, IRepository<Question> questionRepository, ITextAnalysis textAnalysis, IBlobProvider<QuestionAnswerContainer> blobProvider, ICourseRepository courseRepository, IUniversityRepository universityRepository)
        {
            _userRepository = userRepository;
            _questionRepository = questionRepository;
            _textAnalysis = textAnalysis;
            _blobProvider = blobProvider;
            _courseRepository = courseRepository;
            _universityRepository = universityRepository;
        }


        public async Task ExecuteAsync(CreateQuestionCommand message, CancellationToken token)
        {
            var user = await _userRepository.GetRandomFictiveUserAsync(message.Country, token);
            if (user == null)
            {
                throw new InvalidOperationException("we don't have fictive user in that country");
            }


            var Uni = await _universityRepository.GetUniversityByNameAsync(message.University, token);
            if(Uni == null)
            {
                throw new InvalidOperationException("we don't have Universities with the specified name");
            }

            var course = await _courseRepository.GetOrAddAsync(message.CourseName, token);
            var textLanguage = await _textAnalysis.DetectLanguageAsync(message.Text, token);
            var question = new Question(course, message.Text, message.Price, message.Files?.Count() ?? 0,
                user,
                textLanguage, Uni);
           
            await _questionRepository.AddAsync(question, token).ConfigureAwait(true);
            var id = question.Id;

            var l = message.Files?.Select(file => _blobProvider.MoveAsync(file, $"{id}", token)) ??
                    Enumerable.Empty<Task>();
            await Task.WhenAll(l).ConfigureAwait(true);
        }
    }
}