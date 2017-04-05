using System;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ViewModel.Dto.ItemDtos
{
    public class ItemSearchTag
    {
        public string Name { get; set; }

        public TagType Type { get; set; }

        public long Id { get; set; }
    }

    public class FeedSearchTag
    {
        public string Name { get; set; }

        public TagType Type { get; set; }

        public Guid Id { get; set; }
    }
}