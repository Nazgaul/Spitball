using Cloudents.Core.Entities.Db;

namespace Cloudents.Web.Models
{
    public class AddVoteQuestionRequest
    {
        /// <summary>
        /// Question Id
        /// </summary>
        public long QuestionId { get; set; }

        /// <summary>
        /// The vote - None if you want to cancel the vote
        /// </summary>
        public VoteType VoteType { get; set; }
    }
}