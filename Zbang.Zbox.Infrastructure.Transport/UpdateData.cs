using ProtoBuf;
using System;
using System.Text;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class UpdateData : DomainProcess
    {
        protected UpdateData()
        {
           
        }

        public UpdateData(long userId, long boxId,
            long? itemDiscussionId = null,
            long? itemDiscussionReplyId = null,
            Guid? quizDiscussionId = null,
            long? itemId = null,
            Guid? questionId = null,
            Guid? answerId = null,
            long? quizId = null,
            long? flashcardId = null
            )
        {
            UserWhoMadeActionId = userId;
            BoxId = boxId;
            ItemDiscussionId = itemDiscussionId;
            ItemDiscussionReplyId = itemDiscussionReplyId;
            QuizDiscussionId = quizDiscussionId;
            ItemId = itemId;
            QuestionId = questionId;
            AnswerId = answerId;
            QuizId = quizId;
            FlashcardId = flashcardId;
        }
        public override string ProcessResolver => UpdateResolver;

        [ProtoMember(1)]
        public long UserWhoMadeActionId { get; private set; }
        [ProtoMember(2)]
        public long BoxId { get; private set; }
        [ProtoMember(3)]
        public long? ItemId { get; private set; }
        [ProtoMember(4)]
        public Guid? QuestionId { get; private set; }
        [ProtoMember(5)]
        public Guid? AnswerId { get; private set; }
        [ProtoMember(6)]
        public long? QuizId { get; private set; }

        [ProtoMember(7)]
        public long? ItemDiscussionId { get; private set; }
        [ProtoMember(8)]
        public long? ItemDiscussionReplyId { get; private set; }
        [ProtoMember(9)]
        public Guid? QuizDiscussionId { get; private set; }

        [ProtoMember(10)]
        public long? FlashcardId { get; private set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"UserWhoMadeActionId: {UserWhoMadeActionId}");
            sb.AppendLine($"BoxId: {BoxId}");
            sb.AppendLine($"ItemId: {ItemId}");
            sb.AppendLine($"QuestionId: {QuestionId}");
            sb.AppendLine($"AnswerId: {AnswerId}");
            sb.AppendLine($"QuizId: {QuizId}");
            sb.AppendLine($"ItemDiscussionId: {ItemDiscussionId}");
            sb.AppendLine($"ItemDiscussionReplyId: {ItemDiscussionReplyId}");
            sb.AppendLine($"QuizDiscussionId: {QuizDiscussionId}");
            sb.AppendLine($"FlashcardId: {FlashcardId}");
            return sb.ToString();
        }
    }
}
