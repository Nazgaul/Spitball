﻿using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.CommandHandler
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class SetCoursesToUserCommandHandler : ICommandHandler<SetCoursesToUserCommand>
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IRepository<User> _userRepository;

        public SetCoursesToUserCommandHandler(ICourseRepository courseRepository, IRepository<User> userRepository)
        {
            _courseRepository = courseRepository;
            _userRepository = userRepository;
        }


        public async Task ExecuteAsync(SetCoursesToUserCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            user.Courses.Clear();
            foreach (var name in message.Name)
            {
                var course = await _courseRepository.GetOrAddAsync(name, token);
                if (user.Courses.Add(course))
                {
                    course.Count++;
                    await _courseRepository.UpdateAsync(course, token);
                }
            }
            await _userRepository.UpdateAsync(user, token);
        }
    }
}