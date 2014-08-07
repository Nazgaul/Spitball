using System;
using Zbang.Zbox.Domain.Commands.Store;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers.Store
{
    public class AddBannersCommandHandler : ICommandHandler<AddBannersCommand>
    {
        private readonly IRepository<StoreBanner> m_StoreRepository;

        public AddBannersCommandHandler(IRepository<StoreBanner> storeRepository)
        {
            m_StoreRepository = storeRepository;
        }

        public void Handle(AddBannersCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");
            foreach (var bannerStore in message.Banners)
            {
                var banner = m_StoreRepository.Get(bannerStore.Id);
                if (banner == null)
                {
                    banner = new StoreBanner(bannerStore.Id,
                        bannerStore.Url,
                        bannerStore.ImageUrl,
                        GetBannerLocation(bannerStore.Order),
                        bannerStore.Order,
                        bannerStore.UniversityId);
                }
                else
                {
                    banner.Update(bannerStore.Id,
                       bannerStore.Url,
                       bannerStore.ImageUrl,
                       GetBannerLocation(bannerStore.Order),
                       bannerStore.Order, bannerStore.UniversityId);
                }
                m_StoreRepository.Save(banner);

            }
        }

        private StoreBannerLocation GetBannerLocation(int order)
        {
            if (order >= 1 && order <= 9)
            {
                return StoreBannerLocation.TopRight;
            }
            if (order >= 10 && order <= 19)
            {
                return StoreBannerLocation.TopLeft;
            }
            if (order >= 20 && order <= 29)
            {
                return StoreBannerLocation.Center;
            }
            if (order >= 30 && order <= 99)
            {
                return StoreBannerLocation.Product;
            }
            throw new ArgumentException("order not in range", "order");
        }
    }
}
