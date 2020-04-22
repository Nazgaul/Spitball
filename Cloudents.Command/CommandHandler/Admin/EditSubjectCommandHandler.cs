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
        public EditSubjectCommandHandler(IRepository<CourseSubject> subjectRepository )
        {
            _subjectRepository = subjectRepository;
        }

        public async Task ExecuteAsync(EditSubjectCommand message, CancellationToken token)
        {
            var subject = await _subjectRepository.LoadAsync(message.SubjectId, token);
            subject.ChangeName(message.NewName);
            await _subjectRepository.UpdateAsync(subject, token);
        }
    }
}
