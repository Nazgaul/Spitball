using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class DeleteItemCommentCommand : ICommand
    {
        public DeleteItemCommentCommand(long annotationId, long userId)
        {
            AnnotationId = annotationId;
            UserId = userId;
        }
        public long AnnotationId { get; set; }

        public long UserId { get; set; }
    }
}
