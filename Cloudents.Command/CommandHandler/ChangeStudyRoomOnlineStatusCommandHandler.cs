using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler
{
    public class ChangeStudyRoomOnlineStatusCommandHandler : ICommandHandler<ChangeStudyRoomOnlineStatusCommand>
    {
        private readonly IRepository<StudyRoom> _studyRoomRepository;
            


        public ChangeStudyRoomOnlineStatusCommandHandler( IRepository<StudyRoom> studyRoomRepository)
        {
            _studyRoomRepository = studyRoomRepository;
        }

        public async Task ExecuteAsync(ChangeStudyRoomOnlineStatusCommand message, CancellationToken token)
        {
            var studyRoom = await _studyRoomRepository.LoadAsync(message.StudyRoomId, token);
            var studyRoomUser = studyRoom.Users.Single(f => f.User.Id == message.UserId);
            studyRoomUser.Online = message.Status;
        }
    }
}