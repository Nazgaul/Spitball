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

        public async void Run()
        {
            m_KeepRunning = true;
            while (m_KeepRunning)
            {
                var categories = await m_ReadService.GetCategories();
                var storeDto = new List<StoreDto>();
                foreach (var category in categories)
                {
                    storeDto.AddRange(await m_ReadService.ReadData(category));
                }
               
                var products = new List<ProductStore>();
                foreach (var item in storeDto)
                {

                    try
                    {
                        var bytes = await DownloadImage(item.Image);
                        item.Image = await m_BlobProvider.UploadFromLink(bytes, item.Image);
                        products.Add(new ProductStore(item.Id, item.Name, item.ExtraDetails, RandomProvider.GetThreadRandom().Next(15, 50), item.Coupon, item.Saleprice, item.Image));
                    }
                    catch (Exception ex)
                    {
                        TraceLog.WriteError("On hatavot bring image", ex);
                    }
                }
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
