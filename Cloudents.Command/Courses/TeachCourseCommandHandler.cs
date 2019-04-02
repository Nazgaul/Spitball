﻿using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.Courses
{
    public class TeachCourseCommandHandler : ICommandHandler<TeachCourseCommand>
    {
        private readonly IRepository<RegularUser> _userRepository;
        public TeachCourseCommandHandler(IRepository<RegularUser> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(TeachCourseCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            user.UserCourses.Where(w => w.Course.Id == message.Name).FirstOrDefault().CanTeach = true;
            await _userRepository.UpdateAsync(user, token);
        }
    }
}
