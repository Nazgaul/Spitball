﻿using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.Courses
{
    public class UserRemoveCourseCommandHandler : ICommandHandler<UserRemoveCourseCommand>
    {
        private readonly IRegularUserRepository _userRepository;
        private readonly IRepository<Course> _courseRepository;

        public UserRemoveCourseCommandHandler(IRegularUserRepository userRepository, IRepository<Course> courseRepository)
        {
            _userRepository = userRepository;
            _courseRepository = courseRepository;
        }

        public async Task ExecuteAsync(UserRemoveCourseCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            var course = await _courseRepository.LoadAsync(message.Name, token);
            user.RemoveCourse(course);
            await _userRepository.UpdateAsync(user, token);
        }
    }
}