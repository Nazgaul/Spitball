using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class AssignCoursesToTutorCommandHandler : ICommandHandler<AssignCoursesToTutorCommand>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IRepository<RegularUser> _userRepository;
        private readonly IRepository<Tutor> _tutorRepository;

        public AssignCoursesToTutorCommandHandler(ICourseRepository courseRepository,
            IRepository<RegularUser> userRepository, IRepository<Tutor> tutorRepository)
        {
            _courseRepository = courseRepository;
            _userRepository = userRepository;
            _tutorRepository = tutorRepository;
        }


        public async Task ExecuteAsync(AssignCoursesToTutorCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            Tutor tutor = (Tutor)user.UserRoles.FirstOrDefault(f => f is Tutor);
            tutor.Courses.Clear();
            foreach (var name in message.Name)
            {
                var course = await _courseRepository.GetOrAddAsync(name, token);
                if (tutor.Courses.Add(new TutorsCourses(tutor, course)))
                {
                    course.Count++;
                    await _courseRepository.UpdateAsync(course, token);
                }
            }
            
            await _tutorRepository.UpdateAsync(tutor, token);
        }
    }
}
