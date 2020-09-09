using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    public class EnterTailorEdStudyRoomCodeCommandHandler: ICommandHandler<EnterTailorEdStudyRoomCodeCommand>
    {
        private readonly IRepository<TailorEdStudyRoom> _studyRoomRepository;
        private readonly IFictiveUserRepository _userRepository;

        public EnterTailorEdStudyRoomCodeCommandHandler(IRepository<TailorEdStudyRoom> studyRoomRepository, IFictiveUserRepository userRepository)
        {
            _studyRoomRepository = studyRoomRepository;
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(EnterTailorEdStudyRoomCodeCommand message, CancellationToken token)
        {
            var studyRoom = await _studyRoomRepository.LoadAsync(message.StudyRoomId,token);

            if (!studyRoom.Code.Equals(message.Code, StringComparison.Ordinal))
            {
                throw new UnauthorizedAccessException();
            }

            var user = await  _userRepository.GetRandomFictiveUserAsync(studyRoom.Users.Count(), token);
            studyRoom.AddFictiveUser(user);
            message.UserId = user.Id;

        }
    }
}