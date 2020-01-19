using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class CreateQuestionCommandHandler : ICommandHandler<CreateQuestionCommand>
    {

        private readonly IFictiveUserRepository _userRepository;
        private readonly IRepository<Question> _questionRepository;
        private readonly IUniversityRepository _universityRepository;
        private readonly IRepository<Course> _courseRepository;


        public CreateQuestionCommandHandler(IFictiveUserRepository userRepository,
            IRepository<Question> questionRepository,
             IRepository<Course> courseRepository, IUniversityRepository universityRepository)
        {
            _userRepository = userRepository;
            _questionRepository = questionRepository;
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


            var university = await _universityRepository.GetUniversityByNameAndCountryAsync(message.University, message.Country, token);
            if (university == null)
            {
                throw new InvalidOperationException("we don't have Universities with the specified name");
            }

            var course = await _courseRepository.LoadAsync(message.CourseName, token);
            var question = new Question(course, message.Text,
                user,
                university);

            await _questionRepository.AddAsync(question, token);

        }
    }
}