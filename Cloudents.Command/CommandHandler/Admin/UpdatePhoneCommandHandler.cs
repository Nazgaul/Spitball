using Cloudents.Command.Command.Admin;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class UpdatePhoneCommandHandler : ICommandHandler<UpdatePhoneCommand>
    {
        private readonly IRegularUserRepository _repository;
        private readonly IPhoneValidator _phoneValidator;
        public UpdatePhoneCommandHandler(IRegularUserRepository repository, IPhoneValidator phoneValidator)
        {
            _repository = repository;
            _phoneValidator = phoneValidator;
        }

        public async Task ExecuteAsync(UpdatePhoneCommand message, CancellationToken token)
        {
            var user = await _repository.GetAsync(message.UserId, token);
            if (user == null)
            {
                throw new NotFoundException();
            }

            if (string.IsNullOrEmpty(user.Country))
            {
                throw new NullReferenceException();
            }

            var result = await _phoneValidator.ValidateNumberAsync(message.NewPhone,token);
            if (result.phoneNumber == null)
            {
                throw new ArgumentException();
            }
            user.PhoneNumber = result.phoneNumber;
            await _repository.UpdateAsync(user, token);
        }
    }
}
