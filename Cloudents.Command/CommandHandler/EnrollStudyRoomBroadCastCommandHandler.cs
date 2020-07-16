using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    public class EnrollStudyRoomBroadCastCommandHandler : ICommandHandler<EnrollStudyRoomBroadCastCommand>
    {
        private readonly IRegularUserRepository _userRepository;
        private readonly IRepository<BroadCastStudyRoom> _studyRoomRepository;


        public EnrollStudyRoomBroadCastCommandHandler(IRegularUserRepository userRepository,
            IRepository<BroadCastStudyRoom> studyRoomRepository)
        {
            _userRepository = userRepository;
            _studyRoomRepository = studyRoomRepository;
        }

        public async Task ExecuteAsync(EnrollStudyRoomBroadCastCommand message, CancellationToken token)
        {
            //the same as enter study room
            var studyRoom = await _studyRoomRepository.GetAsync(message.StudyRoomId, token);
            if (studyRoom == null)
            {
                throw new NotFoundException();
            }
            

            if (studyRoom.Price.Cents > 0 && studyRoom.Type == StudyRoomType.Broadcast &&
                studyRoom.Tutor.User.SbCountry != Country.Israel && message.Receipt == null)
            {
                if (studyRoom.Tutor.User.Followers
                        .SingleOrDefault(s => s.Follower.Id == message.UserId).Subscriber ==
                    false)
                {
                    throw new UnauthorizedAccessException();
                }
            }
            var user = await _userRepository.LoadAsync(message.UserId, token);

            if (message.Receipt != null)
            {
                studyRoom.AddPayment(user, message.Receipt);
            }

            studyRoom.AddUserToStudyRoom(user);

        }
    }
}