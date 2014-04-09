using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.ViewModel.DTOs.ItemDtos
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
