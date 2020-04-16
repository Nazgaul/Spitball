using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class CreateQuestionCommandHandler : ICommandHandler<CreateQuestionCommand>
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IRegularUserRepository _userRepository;
        private readonly IRepository<Course> _courseRepository;

        public CreateQuestionCommandHandler(IQuestionRepository questionRepository,
            IRegularUserRepository userRepository,
             IRepository<Course> courseRepository)
        {
            _questionRepository = questionRepository;
            _userRepository = userRepository;
            _courseRepository = courseRepository;
        }

        public async Task ExecuteAsync(CreateQuestionCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);

            if (await _questionRepository.GetSimilarQuestionAsync(message.Text, token))
            {
                throw new DuplicateRowException();
            }

            var course = await _courseRepository.LoadAsync(message.Course, token);
            await _courseRepository.UpdateAsync(course, token);


            var question = new Question(
                message.Text,
                user, course);


            await _questionRepository.AddAsync(question, token);
        }
    }
}
