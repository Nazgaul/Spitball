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
        public UpdateData(long userid, long boxid, long? itemId = null, Guid? questionId = null, Guid? answerId = null, long? quizId = null
            )
        {
            UserWhoMadeActionId = userid;
            BoxId = boxid;
            ItemId = itemId;
            QuestionId = questionId;
            AnswerId = answerId;
            QuizId = quizId;
        }
        public override string ProcessResolver
        {
            get { return UpdateResolver; }
        }
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

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("UserWhoMadeActionId: {0}", UserWhoMadeActionId));
            sb.AppendLine(string.Format("BoxId: {0}", BoxId));
            sb.AppendLine(string.Format("ItemId: {0}", ItemId));
            sb.AppendLine(string.Format("QuestionId: {0}", QuestionId));
            sb.AppendLine(string.Format("AnswerId: {0}", AnswerId));
            sb.AppendLine(string.Format("QuizId: {0}", QuizId));
            return base.ToString();
        }
    }
}
