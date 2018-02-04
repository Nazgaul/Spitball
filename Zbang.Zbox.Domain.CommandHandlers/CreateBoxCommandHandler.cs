using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.IdGenerator;
using System;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class CreateBoxCommandHandler : ICommandHandlerAsync<CreateBoxCommand, CreateBoxCommandResult>
    {
        protected readonly IUserRepository UserRepository;
        protected readonly IGuidIdGenerator GuidGenerator;
        protected readonly IQueueProvider QueueProvider;
        private readonly IBoxRepository _boxRepository;

        public CreateBoxCommandHandler(IBoxRepository boxRepository,
            IUserRepository userRepository, IGuidIdGenerator guidGenerator, IQueueProvider queueProvider)
        {
            _boxRepository = boxRepository;
            UserRepository = userRepository;
            GuidGenerator = guidGenerator;
            QueueProvider = queueProvider;
        }

        public virtual async Task<CreateBoxCommandResult> ExecuteAsync(CreateBoxCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (command.BoxName.Length > Box.NameLength)
            {
                throw new OverflowException("Box Name exceed" + Box.NameLength);
            }

            var user = UserRepository.Load(command.UserId);
            var box = _boxRepository.GetBoxWithSameName(command.BoxName.Trim().ToLower(), user);
            if (box != null)
            {
                throw new BoxNameAlreadyExistsException(box.Id);
            }
            box = CreateNewBox(command, user);
            await QueueProvider.InsertFileMessageAsync(new BoxProcessData(box.Id)).ConfigureAwait(true);
            return new CreateBoxCommandResult(box.Id, box.Url);
        }

        private Box CreateNewBox(CreateBoxCommand command, User user)
        {
            var box = new PrivateBox(command.BoxName, user, Infrastructure.Enums.BoxPrivacySetting.AnyoneWithUrl, GuidGenerator.GetId());

            SaveRepositories(user, box);
            box.GenerateUrl();
            _boxRepository.Save(box);
            return box;
        }

        protected void SaveRepositories(User user, Box box)
        {
            UserRepository.Save(user);
            _boxRepository.Save(box, true);
        }
    }
}
