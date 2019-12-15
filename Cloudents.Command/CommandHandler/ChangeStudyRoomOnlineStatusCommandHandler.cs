using System.Linq;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class ChangeStudyRoomOnlineStatusCommandHandler : ICommandHandler<ChangeStudyRoomOnlineStatusCommand>
    {
        private readonly IRepository<StudyRoom> _studyRoomRepository;



        public ChangeStudyRoomOnlineStatusCommandHandler(IRepository<StudyRoom> studyRoomRepository)
        {
            _studyRoomRepository = studyRoomRepository;
        }

        public async Task ExecuteAsync(ChangeStudyRoomOnlineStatusCommand message, CancellationToken token)
        {
            var studyRoom = await _studyRoomRepository.LoadAsync(message.StudyRoomId, token);
            studyRoom.ChangeOnlineStatus(message.UserId, message.Status);

          
            await _studyRoomRepository.UpdateAsync(studyRoom, token);
        }
    }
}