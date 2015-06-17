﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;

namespace Zbang.Zbox.Infrastructure.Search
{
    public interface IItemWriteSearchProvider
    {
        Task<bool> UpdateData(IEnumerable<ItemSearchDto> itemToUpload, IEnumerable<long> itemToDelete);
    }

    public interface IItemWriteSearchProvider2 : IItemWriteSearchProvider
    {
    }

    public interface IItemWriteSearchProvider3 : IItemWriteSearchProvider2
    {
    }
}