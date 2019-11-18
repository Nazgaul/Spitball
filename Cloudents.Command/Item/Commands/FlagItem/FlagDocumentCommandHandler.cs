using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.Item.Commands.FlagItem
{
    public class FlagDocumentCommandHandler : ICommandHandler<FlagDocumentCommand>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Document> _repository;

        public FlagDocumentCommandHandler(IRepository<User> userRepository,
            IRepository<Document> documentRepository)
        {
            _userRepository = userRepository;
            _repository = documentRepository;
        }

        public async Task ExecuteAsync(FlagDocumentCommand message, CancellationToken token)
        {
            var document = await _repository.LoadAsync(message.Id, token);
            BaseUser user = await _userRepository.LoadAsync(message.UserId, token);


            document.Flag(message.FlagReason, user);
            await _repository.UpdateAsync(document, token);
        }
    }
}
