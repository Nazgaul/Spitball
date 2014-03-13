using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class UpdateData : DomainProcess
    {
        protected UpdateData()
        {

        }
        public UpdateData(long userid, long boxid, long? itemId = null, Guid? questionId = null, Guid? answerId = null, long? annotationId = null)
        {
            UserWhoMadeActionId = userid;
            BoxId = boxid;
            ItemId = itemId;
            QuestionId = questionId;
            AnswerId = answerId;
            AnnotationId = annotationId;
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
        public long? AnnotationId { get; private set; }
    }
}
