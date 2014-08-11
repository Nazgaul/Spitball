using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands.Store;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.Store.Dto;
using Zbang.Zbox.Store.Services;
using Zbang.Zbox.ReadServices;

namespace Zbang.Zbox.WorkerRole.Jobs
{
    public class StoreDataSync : IJob
    {
        private bool m_KeepRunning;
        private readonly IReadService m_ReadService;
        private readonly IBlobProductProvider m_BlobProvider;
        private readonly IZboxWriteService m_ZboxWriteService;


        public StoreDataSync(IReadService readService, IBlobProductProvider blobProvider,
            IZboxWriteService zboxWriteService)
        {
            m_ReadService = readService;
            m_BlobProvider = blobProvider;
            m_ZboxWriteService = zboxWriteService;
        }

        public void Run()
        {
            m_KeepRunning = true;
            while (m_KeepRunning)
            {
                BringData();
            }
        }

        private void BringData()
        {
            TraceLog.WriteInfo("Starting to bring data from Hatavot");
            var categoriesDto = m_ReadService.GetCategories();


            var categories = new List<Category>();
            var storeDto = new List<ProductDto>();
            foreach (var category in categoriesDto)
            {
                categories.Add(new Category(category.Id, category.ParentId, category.Name, category.Order));
                var items = m_ReadService.ReadData(category.Id);
                storeDto.AddRange(items);
            }
            try
            {
                var categoriesCommand = new AddCategoriesCommand(categories);
                m_ZboxWriteService.AddCategories(categoriesCommand);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On update categories", ex);
            }
            TraceLog.WriteInfo("build command and download images");
            var products = new List<ProductStore>();
            foreach (var item in storeDto)
            {

                try
                {
                    item.Image = ProcessImage(item.WideImage, item.Image).Result;

                    var upgrades = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>(item.Upgrade1, item.UpgradeValue1),
                            new KeyValuePair<string, string>(item.Upgrade2, item.UpgradeValue2),
                            new KeyValuePair<string, string>(item.Upgrade3, item.UpgradeValue3),
                            new KeyValuePair<string, string>(item.Upgrade4, item.UpgradeValue4),
                            new KeyValuePair<string, string>(item.Upgrade5, item.UpgradeValue5),
                            new KeyValuePair<string, string>(item.Upgrade6, item.UpgradeValue6)
                        };

                    int universityId;
                    int.TryParse(item.UniversityId, out universityId);


                    products.Add(new ProductStore(
                        item.CatalogNumber,
                        item.CategoryCode,
                        item.Coupon,
                        item.DeliveryPrice,
                        item.Description,
                        item.ExtraDetails,
                        item.Featured == "ON",
                        item.Id,
                        item.Name,
                        RandomProvider.GetThreadRandom().Next(15, 50),
                         item.Image,
                         item.ProductPayment,
                         item.Saleprice,
                         item.SupplyTime,
                         item.ProducerName,
                         upgrades,
                         item.NotActive != "ON",
                         TryParseNullableInt(item.UniversityId),
                         item.Order,
                         item.CategoryOrder,
                         item.ProducerId
                         ));
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError("On hatavot bring image", ex);
                }
            }
            try
            {
                var command = new AddProductsToStoreCommand(products);
                m_ZboxWriteService.AddProducts(command);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On update products", ex);
            }
            ProcessBanners();

            Thread.Sleep(TimeSpan.FromMinutes(3));
        }

        private async Task<string> ProcessImage(string wideImage, string image)
        {
            var bytes = await DownloadImage(wideImage, image);
            return await m_BlobProvider.UploadFromLink(bytes, image);
        }

        private async Task<byte[]> DownloadImage(string wideImage, string image)
        {
            if (!string.IsNullOrEmpty(wideImage))
            {
                var wideImagebytes = await DownloadContent("http://www.hatavot.co.il/uploadimages/" + wideImage);
                if (wideImagebytes != null)
                {
                    return wideImagebytes;
                }
            }
            var bytes = await DownloadContent("http://www.hatavot.co.il/uploadimages/250/" + image);
            if (bytes == null)
            {
                throw new NullReferenceException("Couldn't find image for " + image);
            }
            return bytes;
        }

        private void ProcessBanners()
        {
            TraceLog.WriteInfo("Bringing banners");
            var banners = m_ReadService.GetBanners();

            var bannerCommand = new AddBannersCommand(banners.Select(s =>
            {
                var bytes = DownloadContent("http://hatavot.co.il/uploadimages/banners2/" + s.Image).Result;
                var image = m_BlobProvider.UploadFromLink(bytes, s.Image).Result;
                return new Banner(s.Id, s.Url, image, s.Order, s.UniversityId);
            }));
            m_ZboxWriteService.AddBanners(bannerCommand);
        }

        private static int? TryParseNullableInt(string s)
        {
            int f;
            if (int.TryParse(s, out f))
            {
                if (f >= 0) return f;
            }
            return null;
        }

        public void Stop()
        {
            m_KeepRunning = false;
        }

        private async Task<byte[]> DownloadContent(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                throw new ArgumentNullException("imageUrl");
            }
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(imageUrl);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                return null;
            }
        }
    }
}
