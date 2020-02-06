using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class EditSubjectCommandHandler : ICommandHandler<EditSubjectCommand>
    {
        private readonly IRepository<CourseSubject> _subjectRepository;
        private readonly IRepository<CourseSubjectTranslation> _translationrepository;
        private readonly IAdminLanguageRepository _adminLanguageRepository;
        public EditSubjectCommandHandler(IRepository<CourseSubject> subjectRepository,
            IRepository<CourseSubjectTranslation> translationrepository,
             IAdminLanguageRepository adminLanguageRepository)
        {
            _subjectRepository = subjectRepository;
            _translationrepository = translationrepository;
            _adminLanguageRepository = adminLanguageRepository;
        }

        public async Task ExecuteAsync(EditSubjectCommand message, CancellationToken token)
        {
            var subject = await _subjectRepository.LoadAsync(message.SubjectId, token);
            var language = await _adminLanguageRepository.GetLanguageByNameAsync("en", token);
            
            var translation = subject.Translations.Where(w => w.Language == language).FirstOrDefault();
            if (translation != null && !string.IsNullOrEmpty(message.EnSubjectName))
            {
                translation.ChangeName(message.EnSubjectName);
                
            }

            else if(!string.IsNullOrEmpty(message.EnSubjectName))
            {
                var newTranslation = new CourseSubjectTranslation(subject, language, message.EnSubjectName);
                subject.AddTranslation(newTranslation);
                await _translationrepository.AddAsync(newTranslation, token);

            }

            if (subject.Name != message.HeSubjectName && !string.IsNullOrEmpty(message.HeSubjectName))
            {
                subject.ChangeName(message.HeSubjectName);
            }

            await _subjectRepository.UpdateAsync(subject, token);
        }
    }
}
