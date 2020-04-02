using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class PaymentDeclineCommandHandler : ICommandHandler<PaymentDeclineCommand>
    {
        private readonly IRepository<StudyRoomSession> _studyRoomSessionRepository;

        public PaymentDeclineCommandHandler(IRepository<StudyRoomSession> studyRoomSessionRepository)
        {
            _studyRoomSessionRepository = studyRoomSessionRepository;
        }

        public async Task ExecuteAsync(PaymentDeclineCommand message, CancellationToken token)
        {
            var session = await _studyRoomSessionRepository.LoadAsync(message.StudyRoomSessionId, token);
            if (session.StudyRoomVersion.GetValueOrDefault() == StudyRoomSession.StudyRoomNewVersion)
            {
                var sessionUser = session.RoomSessionUsers.AsQueryable().Single(s => s.User.Id == message.UserId);
                sessionUser.NoPay();
            }
            else
            {
                session.SetReceipt("No Pay");
            }

            await _studyRoomSessionRepository.UpdateAsync(session, token);
        }
    }
}