using System;
using System.Diagnostics;
using System.Linq;
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
            var newStoreBanners = message.Banners.ToList();
            foreach (var bannerStore in newStoreBanners)
            {
                var banner = m_StoreRepository.Get(bannerStore.Id);

                Uri u;
                string url = null;

                if (Uri.TryCreate(bannerStore.Url, UriKind.Absolute, out u))
                {
                    url = u.AbsoluteUri;
                }

                if (banner == null)
                {

                    banner = new StoreBanner(bannerStore.Id,
                       url,
                       bannerStore.GetImageUrl(),
                        GetBannerLocation(bannerStore.Order),
                        bannerStore.Order,
                        bannerStore.UniversityId);
                }
                else
                {
                    banner.Update(bannerStore.Id,
                       url,
                       GetBannerLocation(bannerStore.Order),
                       bannerStore.Order, bannerStore.UniversityId);
                }


                m_StoreRepository.Save(banner);

            }
            var x = m_StoreRepository.GetQuerable();
            foreach (var storeBanner in x.ToList())
            {
                if (newStoreBanners.All(a => a.Id != storeBanner.Id))
                {
                    m_StoreRepository.Delete(storeBanner);
                }
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
            if (order >= 30 && order <= 39)
            {
                return StoreBannerLocation.Product;
            }
            if (order >= 40 && order <= 49)
            {
                return StoreBannerLocation.Top;
            }
            throw new ArgumentException("order not in range", "order");
        }
    }
}
