using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.Votes.Commands.AddVoteDocument
{
    public class AddVoteDocumentCommandHandler : BaseAddVoteCommandHandler<Document, long>, ICommandHandler<AddVoteDocumentCommand>
    {
        public AddVoteDocumentCommandHandler(IVoteRepository voteRepository,
            IRepository<RegularUser> userRepository,
            IRepository<Document> questionRepository
            )
            : base(userRepository, voteRepository, questionRepository)
        {
        }

        public async Task ExecuteAsync(AddVoteDocumentCommand message, CancellationToken token)
        {
            await BaseExecuteAsync(message.UserId, message.DocumentId, message.VoteType, token);
            //var user = await _userRepository.LoadAsync(message.UserId, token);
            //if (!Privileges.CanVote(user.Score, message.VoteType))
            //{
            //    throw new UnauthorizedAccessException("not enough score");
            //}
            //var document = await _documentRepository.LoadAsync(message.DocumentId, token);
            //if (document.User.Id == user.Id)
            //{
            //    throw new UnauthorizedAccessException("you cannot vote your own document");

            //}

            //var vote = await _voteRepository.GetVoteDocumentAsync(message.UserId, message.DocumentId, token);
            //if (vote == null && message.VoteType == VoteType.None)
            //{
            //    throw new ArgumentException();
            //}
            //if (vote == null)
            //{
            //    vote = new Vote(user, document, message.VoteType);
            //    document.Item.VoteCount += (int)vote.VoteType;
            //    await _voteRepository.AddAsync(vote, token);
            //    return;
            //}

            //if (message.VoteType == VoteType.None)
            //{
            //    document.Item.VoteCount -= (int)vote.VoteType;
            //    await _voteRepository.DeleteAsync(vote, token);
            //    return;
            //}

            //document.Item.VoteCount -= (int)vote.VoteType;
            //document.Item.VoteCount += (int)message.VoteType;
            //vote.VoteType = message.VoteType;

            //await _voteRepository.UpdateAsync(vote, token);
        }

        protected override Vote CreateVote(RegularUser user, Document question, VoteType vote)
        {
            return new Vote(user, question, vote);
        }

        protected override async Task<Vote> GetVoteAsync(long userId, long id, CancellationToken token)
        {
            return await VoteRepository.GetVoteDocumentAsync(userId, id, token);
        }

        protected override Task ValidateAsync(User user, Document question, CancellationToken token)
        {
            if (question.User.Id == user.Id)
            {
                throw new UnauthorizedAccessException("you cannot vote your own document");

            }

            return Task.CompletedTask;
        }
    }
}