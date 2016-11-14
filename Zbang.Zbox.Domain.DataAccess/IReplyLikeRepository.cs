using System;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public interface IReplyLikeRepository : IRepository<ReplyLike>
    {
        ReplyLike GetUserLike(long userId, Guid replyId);
    }

    //public interface IFlashCardPinRepository : IRepository<FlashcardPin>
    //{
    //    FlashcardPin GetUserPin(long userId, Guid id, int index);
    //}
}