using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Zbang.Zbox.Domain.Commands.Store;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Store.Dto;
using Zbang.Zbox.Store.Services;

namespace Zbang.Zbox.WorkerRole.Jobs
{
    public class StoreDataSync : IJob
    {
        private bool m_KeepRunning;
        private int m_TimeToSyncInSeconds = 60;
        private readonly IReadService m_ReadService;
        private readonly IBlobProductProvider m_BlobProvider;
        private readonly IZboxWriteService m_ZboxWriteService;

        private DateTime m_DateDiff = DateTime.UtcNow.AddYears(-1);


        public StoreDataSync(IReadService readService, IBlobProductProvider blobProvider,
            IZboxWriteService zboxWriteService)
        {
            m_ReadService = readService;
            m_BlobProvider = blobProvider;
            m_ZboxWriteService = zboxWriteService;
            var configurationValueOfHatavot = RoleEnvironment.GetConfigurationSettingValue("SyncHatavotTimeInSeconds");
            int.TryParse(configurationValueOfHatavot, out m_TimeToSyncInSeconds);

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
            var sw = new Stopwatch();
            sw.Start();
            var categoriesDto = m_ReadService.GetCategories();

            var categories = new List<Category>();
            var storeDto = new List<ProductDto>();
            foreach (var category in categoriesDto)
            {
                categories.Add(new Category(category.Id, category.ParentId, category.Name, category.Order));
                var items = m_ReadService.ReadData(category.Id, m_DateDiff);
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
                if (products.Count > 0)
                {
                    var command = new AddProductsToStoreCommand(products);
                    m_ZboxWriteService.AddProducts(command);
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On update products", ex);
            }
            ProcessBanners();
            m_DateDiff = DateTime.UtcNow;
            sw.Stop();
            TraceLog.WriteInfo("sync from hatavot took " + sw.ElapsedMilliseconds + " milliseconds");

            Thread.Sleep(TimeSpan.FromMinutes(m_TimeToSyncInSeconds));
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

            var banners = m_ReadService.GetBanners();


            var bannerCommand = new AddBannersCommand(banners.Select(s =>
            {
                //var bytes = DownloadContent("http://hatavot.co.il/uploadimages/banners2/" + s.Image).Result;
                //var image = m_BlobProvider.UploadFromLink(bytes, s.Image).Result;
                return new Banner(s.Id, s.Url, s.Order, s.UniversityId, () =>
                {
                    var bytes = DownloadContent("http://hatavot.co.il/uploadimages/banners2/" + s.Image).Result;
                    var image = m_BlobProvider.UploadFromLink(bytes, s.Image).Result;
                    return image;
                });
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
