using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class AddPayPalOrderCommandHandler : ICommandHandler<AddPayPalOrderCommand>
    {
        private readonly IRepository<StudyRoomSession> _studyRoomSessionRepository;
        private readonly IRepository<StudyRoom> _studyRoomRepository;
        public AddPayPalOrderCommandHandler(IRepository<StudyRoomSession> studyRoomSessionRepository,
            IRepository<StudyRoom> studyRoomRepository)
        {
            _studyRoomSessionRepository = studyRoomSessionRepository;
            _studyRoomRepository = studyRoomRepository;
        }

        public async Task ExecuteAsync(AddPayPalOrderCommand message, CancellationToken token)
        {
            var room = await _studyRoomRepository.LoadAsync(message.RoomId, token);
            var session = room.GetCurrentSession();
           
            //var session = await _studyRoomSessionRepository.LoadAsync(message.RoomId, token);
            var payPal = new PayPal(message.Token);
            session.SetPyment(payPal);
            await _studyRoomSessionRepository.UpdateAsync(session, token);
        }
    }
}
