using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class AddSubjectToCourseCommandHandler: ICommandHandler<AddSubjectToCourseCommand>
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly ICourseSubjectRepository _subjectRepository;

        public AddSubjectToCourseCommandHandler(IRepository<Course> courseRepository,
            ICourseSubjectRepository subjectRepository)
        {
            _courseRepository = courseRepository;
            _subjectRepository = subjectRepository;
        }

        public async Task ExecuteAsync(AddSubjectToCourseCommand message, CancellationToken token)
        {
            var course = await _courseRepository.LoadAsync(message.CourseName, token);
            var subject = await _subjectRepository.GetCourseSubjectByName(message.Subject
                    , token);
            course.AddSubject(subject);
            await _courseRepository.UpdateAsync(course, token);
        }
    }
}
