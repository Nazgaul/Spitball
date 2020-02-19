using Cloudents.Command.Command.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Command.CommandHandler.Admin
{
    public class UnFlagQuestionCommandHandler : ICommandHandler<UnFlagQuestionCommand>
    {
        private readonly IRepository<Question> _questionRepository;

        public UnFlagQuestionCommandHandler(IRepository<Question> questionRepository)
        {
            _questionRepository = questionRepository;
        }
        public async Task ExecuteAsync(UnFlagQuestionCommand message, CancellationToken token)
        {

            var question = await _questionRepository.LoadAsync(message.QuestionId, token);

            question.UnFlag();
            //if (question.State.FlagReason.Equals(ItemState2.TooManyVotesReason, StringComparison.CurrentCultureIgnoreCase))
            //{
            //    var votes = question.Votes.Where(w => w.Answer == null).ToList();
            //    foreach (var vote in votes)
            //    {
            //        question.Votes.Remove(vote);
            //    }
            //}
            //question.MakePublic();
            //question.VoteCount = question.Votes.Count;

            await _questionRepository.UpdateAsync(question, token);
        }
    }
}
