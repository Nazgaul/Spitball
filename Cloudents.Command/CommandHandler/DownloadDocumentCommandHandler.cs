using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler
{
    public class DownloadDocumentCommandHandler : ICommandHandler<DownloadDocumentCommand>
    {
        private readonly IRepository<BaseUser> _userRepository;
        private readonly IRegularUserRepository _regularUserRepository;
        private readonly IRepository<Document> _documentRepository;
        public DownloadDocumentCommandHandler(IRepository<BaseUser> userRepository, IRepository<Document> documentRepository, IRegularUserRepository regularUserRepository)
        {
            _userRepository = userRepository;
            _documentRepository = documentRepository;
            _regularUserRepository = regularUserRepository;
        }

        public async Task ExecuteAsync(DownloadDocumentCommand message, CancellationToken token)
        {
            var document = await _documentRepository.LoadAsync(message.DocumentId, token);
            var user = await _regularUserRepository.LoadAsync(message.UserId, token);
            document.User.AddFollower(user);
            document.AddDownload(user);
            await _userRepository.UpdateAsync(document.User, token);
            await _documentRepository.UpdateAsync(document, token);
        }
    }
}
