using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class CreateQuestionCommandHandler : ICommandHandler<CreateQuestionCommand>
    {

        private readonly IFictiveUserRepository _userRepository;
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<University> _universityRepository;
        private readonly ITextAnalysis _textAnalysis;
        private readonly IRepository<Course> _courseRepository;


        public CreateQuestionCommandHandler(IFictiveUserRepository userRepository,
            IRepository<Question> questionRepository, ITextAnalysis textAnalysis,
             IRepository<Course> courseRepository, IRepository<University> universityRepository)
        {
            _userRepository = userRepository;
            _questionRepository = questionRepository;
            _textAnalysis = textAnalysis;
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


            var university = await _universityRepository.LoadAsync(message.University, token);
            if(university == null)
            {
                throw new InvalidOperationException("we don't have Universities with the specified name");
            }

            var course = await _courseRepository.LoadAsync(message.CourseName, token);
            var textLanguage = await _textAnalysis.DetectLanguageAsync(message.Text, token);
            var question = new Question(course, message.Text, 
                user,
                textLanguage, university);
           
            await _questionRepository.AddAsync(question, token);

        }
    }
}