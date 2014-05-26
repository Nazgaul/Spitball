using System;

namespace Zbang.Zbox.ViewModel.DTOs.ItemDtos
{
    public class FileDto : ItemDto
    {
        public FileDto(long id, string name, long ownerId,
            string thumbnail,
            string tabId, int numOfViews, float rate, bool sponsored, string owner, string description, int numOfDownloads,
            DateTime date, int commentsCount)
            : base(id, name, ownerId,
             tabId, numOfViews, rate, thumbnail, sponsored, owner, date)
        {
            NumOfDownloads = numOfDownloads;
            Description = description;
            CommentsCount = commentsCount;
        }
        public override string Type
        {
            get { return "File"; }
        }
        public int NumOfDownloads { get; private set; }
        public string Description { get; private set; }
        public int CommentsCount { get; private set; }
    }
}
