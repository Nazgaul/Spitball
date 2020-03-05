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
        public AddPayPalOrderCommandHandler(IRepository<StudyRoomSession> studyRoomSessionRepository)
        {
            _studyRoomSessionRepository = studyRoomSessionRepository;
        }

        public async Task ExecuteAsync(AddPayPalOrderCommand message, CancellationToken token)
        {
            var session = await _studyRoomSessionRepository.LoadAsync(message.RoomId, token);
            var payPal = new PayPal(message.Token);
            session.SetPyment(payPal);
            await _studyRoomSessionRepository.UpdateAsync(session, token);
        }
    }
}
