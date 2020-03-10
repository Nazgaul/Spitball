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
            var session = await _studyRoomRepository.LoadAsync(message.SessionId, token);
            var v = await _payPalService.GetPaymentAsync(message.Token, token);
            var user = await _userRepository.LoadAsync(message.UserId, token);
            user.AddToken(message.Token, v.Amount);
            user.UseCoupon(session.Tutor);
            await _userRepository.UpdateAsync(user, token);
        }
    }
}
