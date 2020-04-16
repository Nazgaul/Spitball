//using Cloudents.Command.Command.Admin;
//using Cloudents.Core.Entities;
//using Cloudents.Core.Interfaces;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Cloudents.Command.CommandHandler.Admin
//{
//    public class DeleteSubjectCommandHandler : ICommandHandler<DeleteSubjectCommand>
//    {
//        private readonly IRepository<CourseSubject> _subjectRepository;
//        private readonly ICourseRepository _courseRepository;
//        public DeleteSubjectCommandHandler(IRepository<CourseSubject> subjectRepository,
//            ICourseRepository courseRepository)
//        {
//            _subjectRepository = subjectRepository;
//            _courseRepository = courseRepository;
//        }

//        public async Task ExecuteAsync(DeleteSubjectCommand message, CancellationToken token)
//        {
//            var courses = await _courseRepository.GetCoursesBySubjectIdAsync(message.SubjectId, token);
//            foreach (var course in courses)
//            {
//                course.SetSubject(null);
//                await _courseRepository.UpdateAsync(course, token);
//            }
//            var subject = await _subjectRepository.GetAsync(message.SubjectId, token);
//            await _subjectRepository.DeleteAsync(subject, token);
//        }
//    }
//}
