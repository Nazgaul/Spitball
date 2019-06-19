using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

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

            //if (!Privileges.CanVote(user.Score, type))
            //{
            //    throw new NoEnoughScoreException();
            //}
            var document = await _repository.LoadAsync(message.DocumentId, token);
            document.Vote(message.VoteType,user);
            await _repository.UpdateAsync(document, token);
            //if (question.State != ItemState.Ok)
            //{
            //    throw new NotFoundException();
            //}
            //await ValidateAsync(user, question, token);
            //var vote = await GetVoteAsync(message.UserId, message.DocumentId, token);

            //if (vote == null && message.VoteType == VoteType.None)
            //{
            //    throw new ArgumentException();
            //}
            //if (vote == null)
            //{
            //    //vote = new Vote(user, question, message.VoteType);
            //    vote = CreateVote(user, question, message.VoteType);
            //    await _voteRepository.AddAsync(vote, token);

            //    question.VoteCount += (int)vote.VoteType;

            //    if (question.VoteCount < AddVoteAnswerCommandHandler.VotesToFlag)
            //    {
            //        question.Flag("Too many down vote", null);
            //        // _eventStore.Add(new ItemFlaggedEvent(question));
            //    }
            //    return;
            //}

            //if (message.VoteType == VoteType.None)
            //{
            //    question.VoteCount -= (int)vote.VoteType;
            //    if (question.VoteCount < AddVoteAnswerCommandHandler.VotesToFlag)
            //    {
            //        question.Flag("Too many down vote", null);
            //        // _eventStore.Add(new ItemFlaggedEvent(question));
            //    }
            //    await _voteRepository.DeleteAsync(vote, token);
            //    return;
            //}

            //question.VoteCount -= (int)vote.VoteType;
            //question.VoteCount += (int)message.VoteType;
            //if (question.VoteCount < AddVoteAnswerCommandHandler.VotesToFlag)
            //{
            //    question.Flag("Too many down vote", null);
            //    // _eventStore.Add(new ItemFlaggedEvent(question));
            //}
            //vote.VoteType = message.VoteType;

            //await _voteRepository.UpdateAsync(vote, token);
        }

        //protected  Vote CreateVote(RegularUser user, Document question, VoteType vote)
        //{
        //    return new Vote(user, question, vote);
        //}

        //protected  async Task<Vote> GetVoteAsync(long userId, long id, CancellationToken token)
        //{
        //    return await _voteRepository.GetVoteDocumentAsync(userId, id, token);
        //}

        //protected  Task ValidateAsync(User user, Document question, CancellationToken token)
        //{
        //    if (question.User.Id == user.Id)
        //    {
        //        throw new UnauthorizedAccessException("you cannot vote your own document");

        //    }

        //    return Task.CompletedTask;
        //}
    }
}