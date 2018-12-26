using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Event;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;

namespace Cloudents.Command.Votes.Commands
{
    public abstract class BaseAddVoteCommandHandler<T, TId> where T : ItemObject
    {
        private const int VotesToFlag = -2;
        private readonly IRepository<RegularUser> _userRepository;
        protected readonly IVoteRepository VoteRepository;
        private readonly IRepository<T> _repository;
        private readonly IEventStore _eventStore;

        protected BaseAddVoteCommandHandler(IRepository<RegularUser> userRepository, IVoteRepository voteRepository, IRepository<T> repository, IEventStore eventStore)
        {
            _userRepository = userRepository;
            VoteRepository = voteRepository;
            _repository = repository;
            _eventStore = eventStore;
        }

        protected async Task BaseExecuteAsync(long userId, TId id, VoteType type, CancellationToken token)
        {
            var user = await _userRepository.LoadAsync(userId, token);

            //if (!Privileges.CanVote(user.Score, type))
            //{
            //    throw new NoEnoughScoreException();
            //}
            var question = await _repository.LoadAsync(id, token);
            if (question.State != ItemState.Ok)
            {
                throw new NotFoundException();
            }
            await ValidateAsync(user, question, token);
            var vote = await GetVoteAsync(userId, id, token);

            if (vote == null && type == VoteType.None)
            {
                throw new ArgumentException();
            }
            if (vote == null)
            {
                //vote = new Vote(user, question, message.VoteType);
                vote = CreateVote(user, question, type);
                await VoteRepository.AddAsync(vote, token);

                question.VoteCount += (int)vote.VoteType;
                
                if (question.VoteCount < VotesToFlag)
                {
                    question.Flag("Too many down vote", null);
                    // _eventStore.Add(new ItemFlaggedEvent(question));
                }
                return;
            }

            if (type == VoteType.None)
            {
                question.VoteCount -= (int)vote.VoteType;
                if (question.VoteCount < VotesToFlag)
                {
                    question.Flag("Too many down vote", null);
                    // _eventStore.Add(new ItemFlaggedEvent(question));
                }
                await VoteRepository.DeleteAsync(vote, token);
                return;
            }

            question.VoteCount -= (int)vote.VoteType;
            question.VoteCount += (int)type;
            if (question.VoteCount < VotesToFlag)
            {
                question.Flag("Too many down vote", null);
                // _eventStore.Add(new ItemFlaggedEvent(question));
            }
            vote.VoteType = type;

            await VoteRepository.UpdateAsync(vote, token);
        }

        protected abstract Vote CreateVote(RegularUser user, T question, VoteType vote);

        protected abstract Task ValidateAsync(User user, T answer, CancellationToken token);
        protected abstract Task<Vote> GetVoteAsync(long userId, TId id, CancellationToken token);


    }
}