using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
  public  class DeleteItemCommentReplyCommand : ICommand
    {
      public DeleteItemCommentReplyCommand(long userId, long replyId)
      {
          ReplyId = replyId;
          UserId = userId;
      }

      public long ReplyId { get;private set; }

        public long UserId { get; private set; }
    }
}
