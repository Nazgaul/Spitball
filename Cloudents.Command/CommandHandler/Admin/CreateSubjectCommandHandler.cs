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
        private readonly IAdminLanguageRepository _adminLanguageRepository;
        public CreateSubjectCommandHandler(IRepository<CourseSubject> subjectRepository,
            IAdminLanguageRepository adminLanguageRepository)
        {
            _subjectRepository = subjectRepository;
            _adminLanguageRepository = adminLanguageRepository;
        }

        public async Task ExecuteAsync(CreateSubjectCommand message, CancellationToken token)
        {
            var subject = new CourseSubject(message.HeSubjectName);
            var language = await _adminLanguageRepository.GetLanguageByNameAsync("en", token);
            
            if (!string.IsNullOrEmpty(message.EnSubjectName))
            {
                var translation = new CourseSubjectTranslation(subject, language, message.EnSubjectName);
                subject.AddTranslation(translation);
            }

            await _subjectRepository.AddAsync(subject, token);
        }
    }
}
