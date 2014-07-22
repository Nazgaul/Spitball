using System;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public interface IItemDto
    {
        string Url { get; set; }
        string UserUrl { get; set; }
        string DownloadUrl { get; set; }
        long Id { get; set; }
        long OwnerId { get; set; }
        string Owner { get; set; }
        string Name { get; set; }
        DateTime Date { get; set; }
    }
}
