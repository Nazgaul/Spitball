using System;
using System.Collections.Generic;
using Zbang.Zbox.Store.Dto;

namespace Zbang.Zbox.Store.Services
{
    public interface IReadService
    {
        IEnumerable<ProductDto> ReadData(IEnumerable<int> categories, DateTime diffTime);
        IEnumerable<CategoryDto> GetCategories();

        IEnumerable<BannerDto> GetBanners();
    }
}
