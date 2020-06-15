using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
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
            var studyRoom = await _studyRoomRepository.LoadAsync(message.StudyRoomId, token);
            var user = await _userRepository.LoadAsync(message.UserId, token);

            studyRoom.AddUserToStudyRoom(user);
           
        }
    }
}