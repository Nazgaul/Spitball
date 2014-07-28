using System.Collections.Generic;
using Zbang.Zbox.Store.Dto;

namespace Zbang.Zbox.Store.Services
{
    public interface IReadService
    {
        IEnumerable<ProductDto> ReadData(int category);
        IEnumerable<CategoryDto> GetCategories();

        IEnumerable<BannerDto> GetBanners();
    }
}
