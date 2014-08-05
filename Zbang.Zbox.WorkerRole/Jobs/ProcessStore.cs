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
    public class ProcessStore : IJob
    {
        private bool m_KeepRunning;
        private bool m_BringData;
        private readonly IReadService m_ReadService;
        private readonly IWriteService m_WriteService;
        private readonly IBlobProductProvider m_BlobProvider;
        private readonly IZboxWriteService m_ZboxWriteService;
        private readonly IZboxReadService m_ZboxReadService;
        private readonly QueueProcess m_QueueProcess;


        public ProcessStore(IReadService readService, IBlobProductProvider blobProvider,
            IZboxWriteService zboxWriteService,
            IQueueProvider queueProvider,
            IWriteService writeService,
            IZboxReadService zboxReadService)
        {
            m_ReadService = readService;
            m_BlobProvider = blobProvider;
            m_ZboxWriteService = zboxWriteService;
            m_QueueProcess = new QueueProcess(queueProvider, TimeSpan.FromSeconds(60));
            m_WriteService = writeService;
            m_ZboxReadService = zboxReadService;
        }

        public void Run()
        {
            m_KeepRunning = true;
            while (m_KeepRunning)
            {
                if (!m_BringData)
                {
                    Task.Factory.StartNew(BringData);
                }
                m_QueueProcess.RunQueue(new OrderQueueName(), msg =>
                {
                    var order = msg.FromMessageProto<StoreOrderData>();
                    var productDetail = m_ZboxReadService.GetProductCheckOut(new ViewModel.Queries.GetStoreProductQuery(order.ProdcutId)).Result;

                    var features = new KeyValuePair<string, string>[6];
                    int index = 0;
                    //order.Features
                    foreach (var feature in productDetail.Features.Where(w => order.Features.Contains(w.Id)))
                    {
                        features[index] = new KeyValuePair<string, string>(feature.Category,
                            feature.Description + "*" + feature.Price + "*");
                        index++;
                    }
                    for (int i = index; i < 6; i++)
                    {
                        features[i] = new KeyValuePair<string, string>(string.Empty, string.Empty);
                    }
                    //TODO: need to add email


                    m_WriteService.InsertOrder(new OrderSubmitDto(
                        order.ProdcutId,
                        order.IdentificationNumber,
                        order.FirstName,
                        order.LastName,
                        order.Address,
                        order.CardHolderIdentificationNumber,
                        order.Notes,
                        order.City,
                        order.CreditCardNameHolder,
                        order.CreditCardNumber,
                        order.CreditCardExpiration,
                        order.Cvv,
                        order.UniversityId,
                        features[0].Key, features[0].Value,
                        features[1].Key, features[1].Value,
                        features[2].Key, features[2].Value,
                        features[3].Key, features[3].Value,
                        features[4].Key, features[4].Value,
                        features[5].Key, features[5].Value,
                        order.Email,
                        order.Phone,
                        order.Phone2,
                        0,
                        order.NumberOfPayment));
                    return true;
                }, TimeSpan.FromMinutes(1), int.MaxValue);
            }
        }

        private void BringData()
        {
            m_BringData = true;
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
                    var bytes = DownloadImage("http://www.hatavot.co.il/uploadimages/250/" + item.Image).Result;
                    item.Image = m_BlobProvider.UploadFromLink(bytes, item.Image).Result;

                    var upgrades = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>(item.Upgrade1, item.UpgradeValue1),
                            new KeyValuePair<string, string>(item.Upgrade2, item.UpgradeValue2),
                            new KeyValuePair<string, string>(item.Upgrade3, item.UpgradeValue3),
                            new KeyValuePair<string, string>(item.Upgrade4, item.UpgradeValue4),
                            new KeyValuePair<string, string>(item.Upgrade5, item.UpgradeValue5),
                            new KeyValuePair<string, string>(item.Upgrade6, item.UpgradeValue6)
                        };



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
                         upgrades
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

            TraceLog.WriteInfo("Bringing banners");
            var banners = m_ReadService.GetBanners();

            var bannerCommand = new AddBannersCommand(banners.Select(s =>
            {
                var bytes = DownloadImage("http://hatavot.co.il/uploadimages/banners2/" + s.Image).Result;
                var image = m_BlobProvider.UploadFromLink(bytes, s.Image).Result;
                return new Banner(s.Id, s.Url, image, s.Order);
            }));
            m_ZboxWriteService.AddBanners(bannerCommand);


            Thread.Sleep(TimeSpan.FromDays(1));
            m_BringData = false;
        }

        public void Stop()
        {
            m_KeepRunning = false;
        }

        private async Task<byte[]> DownloadImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                throw new ArgumentNullException("imageUrl");
            }
            using (var httpClient = new HttpClient())
            {
                return await httpClient.GetByteArrayAsync(imageUrl);
            }
        }
    }
}
