using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.StudyRooms
{
    public class EnterStudyRoomCommandHandler : ICommandHandler<EnterStudyRoomCommand>
    {
        private readonly IRepository<StudyRoom> _studyRoomRepository;
        private readonly IRegularUserRepository _userRepository;

        public EnterStudyRoomCommandHandler(IRepository<StudyRoom> studyRoomRepository, IRegularUserRepository userRepository)
        {
            _studyRoomRepository = studyRoomRepository;
            _userRepository = userRepository;
        }

        public async Task ExecuteAsync(EnterStudyRoomCommand message, CancellationToken token)
        {
            var studyRoom = await _studyRoomRepository.GetAsync(message.StudyRoomId, token);
            if (studyRoom == null)
            {
                throw new ArgumentNullException();
            }

            if (studyRoom.StudyRoomType == StudyRoomType.Private)
            {
                if (studyRoom.Tutor.User.Id == message.UserId)
                {
                    return;
                }
                var _ = studyRoom.Users.AsQueryable().Single(s => s.User.Id == message.UserId);
                return;
            }

            var user = await _userRepository.LoadAsync(message.UserId, token);
            studyRoom.AddUserToStudyRoom(user);
        }
    }
}