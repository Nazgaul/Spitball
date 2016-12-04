using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    [ProtoInclude(11,typeof(RegisterBadgeData))]
    [ProtoInclude(12,typeof(FollowClassBadgeData))]
    [ProtoInclude(13,typeof(CreateQuizzesBadgeData))]
    [ProtoInclude(14,typeof(UploadItemsBadgeData))]
    [ProtoInclude(15,typeof(LikesBadgeData))]
    public abstract class BadgeData : DomainProcess
    {
        protected BadgeData()
        {
        }

        protected BadgeData(long userId/*, int badge*/)
        {
            UserId = userId ;
           // Badge = badge;
        }

        

        [ProtoMember(1)]
        public long UserId { get; private set; }
        //[ProtoMember(2)]
        //public int Badge { get; private set; }
        public override string ProcessResolver => BadgeResolver;
    }
    [ProtoContract]
    public class RegisterBadgeData : BadgeData
    {
        protected RegisterBadgeData()
        {
        }

        public RegisterBadgeData(long userId)
            :base(userId)
        {
           
        }
        
    }
    [ProtoContract]
    public class FollowClassBadgeData : BadgeData
    {
        protected FollowClassBadgeData()
        {
        }

        public FollowClassBadgeData(long userId)
            : base(userId)
        {

        }

    }
    [ProtoContract]
    public class CreateQuizzesBadgeData : BadgeData
    {
        protected CreateQuizzesBadgeData()
        {
        }

        public CreateQuizzesBadgeData(long userId)
            : base(userId)
        {

        }

    }
    [ProtoContract]
    public class UploadItemsBadgeData : BadgeData
    {
        protected UploadItemsBadgeData()
        {
        }

        public UploadItemsBadgeData(long userId)
            : base(userId)
        {

        }

    }
    [ProtoContract]
    public class LikesBadgeData : BadgeData
    {
        protected LikesBadgeData()
        {
        }

        public LikesBadgeData(long userId)
            : base(userId)
        {

        }

    }
}