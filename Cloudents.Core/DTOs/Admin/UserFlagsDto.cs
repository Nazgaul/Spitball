//using Cloudents.Core.Attributes;
//using Cloudents.Core.Entities;
//using System;

//namespace Cloudents.Core.DTOs.Admin
//{
//    public class UserFlagsDto
//    {
//        [EntityBind(nameof(Document.Name), nameof(Question.Text), nameof(Answer.Text))]
//        public string Text { get; set; }
//        [EntityBind(nameof(Document.TimeStamp), nameof(Question.Created), nameof(Answer.Created))]
//        public DateTime Created { get; set; }
//        [EntityBind(nameof(Document.Status.State),nameof(Question.Status.State),nameof(Answer.Status.State))]
//        public string State { get; set; }
//        [EntityBind(nameof(Document.Status.FlagReason),nameof(Question.Status.FlagReason),nameof(Answer.Status.FlagReason))]
//        public string FlagReason { get; set; }
//        [EntityBind(nameof(Document.VoteCount),nameof(Question.VoteCount),nameof(Answer.VoteCount))]
//        public int VoteCount { get; set; }
//        public string ItemType { get; set; }
//    }
//}
