using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class AddPayPalOrderCommandHandler : ICommandHandler<AddPayPalOrderCommand>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IPayPalService _payPalService;
        private readonly IRepository<StudyRoom> _studyRoomRepository;

        public AddPayPalOrderCommandHandler(IRepository<User> userRepository, IPayPalService payPalService, IRepository<StudyRoom> studyRoomRepository)
        {
            _userRepository = userRepository;
            _payPalService = payPalService;
            _studyRoomRepository = studyRoomRepository;
        }

        public async Task ExecuteAsync(AddPayPalOrderCommand message, CancellationToken token)
        {
            var studyRoom = await _studyRoomRepository.LoadAsync(message.SessionId, token);
            var (authorizationId, amount) = await _payPalService.AuthorizationOrderAsync(message.PayPalOrderId, token);
            var user = await _userRepository.LoadAsync(message.UserId, token);
            user.AddPaymentToken(message.PayPalOrderId, authorizationId, amount, studyRoom);

            await _userRepository.UpdateAsync(user, token);
        }
    }
}
