using System;
using Cloudents.Core.Entities;

namespace Cloudents.Web.Models
{
    public class AddVoteQuestionRequest
    {
        /// <summary>
        /// Question Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The vote - None if you want to cancel the vote
        /// </summary>
        public VoteType VoteType { get; set; }
    }


    public class AddVoteAnswerRequest
    {
        /// <summary>
        /// Answer Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The vote - None if you want to cancel the vote
        /// </summary>
        public VoteType VoteType { get; set; }
    }
}