using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands.Store;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.Store.Dto;
using Zbang.Zbox.Store.Services;

namespace Zbang.Zbox.WorkerRole.Jobs
{
    public class SyncHatavotProduct : IJob
    {
        private bool m_KeepRunning;
        private readonly IReadService m_ReadService;
        private readonly IBlobProductProvider m_BlobProvider;
        private readonly IZboxWriteService m_ZboxWriteService;


        public SyncHatavotProduct(IReadService readService, IBlobProductProvider blobProvider, IZboxWriteService zboxWriteService)
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
                TraceLog.WriteInfo("build command and download images");
                var products = new List<ProductStore>();
                foreach (var item in storeDto)
                {

                    try
                    {
                        var bytes = DownloadImage(item.Image).Result;
                        item.Image = m_BlobProvider.UploadFromLink(bytes, item.Image).Result;
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
                             item.ProducerName
                            ));
                    }
                    catch (Exception ex)
                    {
                        TraceLog.WriteError("On hatavot bring image", ex);
                    }
                }
                var categoriesCommand = new AddCategoriesCommand(categories);
                m_ZboxWriteService.AddCategories(categoriesCommand);
                var command = new AddProductsToStoreCommand(products);
                m_ZboxWriteService.AddProducts(command);
                Thread.Sleep(TimeSpan.FromDays(1));
            }
        }

        public void Stop()
        {
            m_KeepRunning = false;
        }

        private async Task<byte[]> DownloadImage(string imageName)
        {
            if (string.IsNullOrEmpty(imageName))
            {
                throw new ArgumentNullException("imageName");
            }
            using (var httpClient = new HttpClient())
            {
                return await httpClient.GetByteArrayAsync("http://www.hatavot.co.il/uploadimages/250/" + imageName);
            }
        }
    }
}
