using System;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Votes.Commands.AddVoteDocument
{
    public class AddVoteDocumentCommandHandler : ICommandHandler<AddVoteDocumentCommand>
    {
        private readonly IVoteRepository _voteRepository;
        private readonly IRepository<RegularUser> _userRepository;
        private readonly IRepository<Document> _documentRepository;

        public AddVoteDocumentCommandHandler(IVoteRepository voteRepository,
            IRepository<RegularUser> userRepository, IRepository<Document> documentRepository)
        {
            _voteRepository = voteRepository;
            _userRepository = userRepository;
            _documentRepository = documentRepository;
        }

        public async Task ExecuteAsync(AddVoteDocumentCommand message, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(message.UserId, token);
            if (!Privileges.CanVote(user.Score, message.VoteType))
            {
                throw new UnauthorizedAccessException("not enough score");
            }
            var document = await _documentRepository.LoadAsync(message.DocumentId, token);
            if (document.User.Id == user.Id)
            {
                throw new UnauthorizedAccessException("you cannot vote your own document");

            }

            var vote = await _voteRepository.GetVoteDocumentAsync(message.UserId, message.DocumentId, token);
            if (vote == null && message.VoteType == VoteType.None)
            {
                throw new ArgumentException();
            }
            if (vote == null)
            {
                vote = new Vote(user, document, message.VoteType);
                document.Item.VoteCount += (int)vote.VoteType;
                await _voteRepository.AddAsync(vote, token);
                return;
            }

            if (message.VoteType == VoteType.None)
            {
                document.Item.VoteCount -= (int)vote.VoteType;
                await _voteRepository.DeleteAsync(vote, token);
                return;
            }

            document.Item.VoteCount -= (int)vote.VoteType;
            document.Item.VoteCount += (int)message.VoteType;
            vote.VoteType = message.VoteType;

            await _voteRepository.UpdateAsync(vote, token);
        }
    }
}