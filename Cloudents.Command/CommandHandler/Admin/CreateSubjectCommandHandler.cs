using System.Runtime.InteropServices;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class CreateSubjectCommandHandler : ICommandHandler<CreateSubjectCommand>
    {
        private readonly IRepository<CourseSubject> _subjectRepository;
        private readonly IRepository<AdminUser> _userRepository;


        public CreateSubjectCommandHandler(IRepository<CourseSubject> subjectRepository, IRepository<AdminUser> userRepository)
        {
            _subjectRepository = subjectRepository;
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(CreateSubjectCommand message, CancellationToken token)
        {
            var adminUser = await _userRepository.LoadAsync(message.UserId, token);
            var subject = new CourseSubject(message.Name, adminUser.SbCountry ?? message.Country);
            await _subjectRepository.AddAsync(subject, token);
        }
    }
}
