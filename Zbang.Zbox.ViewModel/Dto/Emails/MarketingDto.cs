using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto.Emails
{
    public class MarketingDto
    {
        public string Email { get; set; }
        public string Culture { get; set; }
        public string Name { get; set; }
    }

    public abstract class LikesDto : MarketingDto
    {
        public string LikePersonName { get; set; }
        public string OnElement { get; set; }
        public long UserId { get; set; }
        public abstract LikeType LikeType { get; }
    }

    public class ItemLikesDto : LikesDto
    {
        public override LikeType LikeType => LikeType.Item;
    }

    public class CommentLikesDto : LikesDto
    {
        public override LikeType LikeType => LikeType.Comment;
    }

    public class ReplyLikesDto : LikesDto
    {
        public override LikeType LikeType => LikeType.Reply;
    }
}
