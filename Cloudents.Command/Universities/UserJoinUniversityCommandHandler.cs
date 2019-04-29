﻿using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.Universities
{
    public class UserJoinUniversityCommandHandler : ICommandHandler<UserJoinUniversityCommand>
    {
        private readonly IRepository<RegularUser> _userRepository;
        private readonly IRepository<University> _universityRepository;

        public UserJoinUniversityCommandHandler(IRepository<RegularUser> userRepository, IRepository<University> universityRepository)
        {
            _userRepository = userRepository;
            _universityRepository = universityRepository;
        }

        public async Task ExecuteAsync(UserJoinUniversityCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);

            var university = await _universityRepository.LoadAsync(message.UniversityId, token);
            user.SetUniversity(university);
            await _userRepository.UpdateAsync(user, token);
        }
    }
}