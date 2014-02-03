using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class ChangeFileNameCommand : ICommand
    {
        public ChangeFileNameCommand(long fileId, string newFileName, long userId)
        {
            FileId = fileId;
            NewFileName = newFileName;
            UserId = userId;
        }
        public long FileId { get; set; }
        public long UserId { get; set; }
        public string NewFileName { get; set; }
       
    }
}
