using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.Votes.Commands.AddVoteDocument
{
    public class AddVoteDocumentCommandHandler : ICommandHandler<AddVoteDocumentCommand>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Document> _repository;

        public AddVoteDocumentCommandHandler(
            IRepository<User> userRepository,
            IRepository<Document> questionRepository
            )

        {
            _userRepository = userRepository;
            _repository = questionRepository;
        }

        public async Task ExecuteAsync(AddVoteDocumentCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);

            var document = await _repository.LoadAsync(message.DocumentId, token);
            document.Vote(message.VoteType, user);
            await _repository.UpdateAsync(document, token);

        }


    }
}