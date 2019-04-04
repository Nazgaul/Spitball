﻿using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.Courses
{
    public class CreateCourseCommandHandler : ICommandHandler<CreateCourseCommand>
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<RegularUser> _userRepository;

        public CreateCourseCommandHandler(IRepository<Course> courseRepository, IRepository<RegularUser> userRepository)
        {
            _courseRepository = courseRepository;
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(CreateCourseCommand message, CancellationToken token)
        {
            var course = new Course(message.Name);
            await _courseRepository.AddAsync(course, token);
            var user = await _userRepository.LoadAsync(message.UserId, token);
            user.AssignCourses(new []{course});
        }
    }
}