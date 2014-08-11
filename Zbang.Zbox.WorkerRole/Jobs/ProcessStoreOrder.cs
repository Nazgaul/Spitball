using System;
using System.Linq;
using System.Collections.Generic;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Mail.EmailParameters;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.Store.Dto;
using Zbang.Zbox.Store.Services;
using Zbang.Zbox.ReadServices;

namespace Zbang.Zbox.WorkerRole.Jobs
{
    public class ProcessStoreOrder : IJob
    {
        private bool m_KeepRunning;
        private readonly IWriteService m_WriteService;
        private readonly IZboxReadService m_ZboxReadService;
        private readonly IMailComponent m_MailComponent;
        private readonly QueueProcess m_QueueProcess;


        public ProcessStoreOrder(
            IQueueProvider queueProvider,
            IWriteService writeService,
            IZboxReadService zboxReadService, IMailComponent mailComponent)
        {
            m_QueueProcess = new QueueProcess(queueProvider, TimeSpan.FromSeconds(60));
            m_WriteService = writeService;
            m_ZboxReadService = zboxReadService;
            m_MailComponent = mailComponent;
        }

        public void Run()
        {
            m_KeepRunning = true;
            while (m_KeepRunning)
            {
                m_QueueProcess.RunQueue(new OrderQueueName(), msg =>
                {
                    var storeData = msg.FromMessageProto<StoreData>();
                    var order = storeData as StoreOrderData;
                    if (order != null)
                    {
                        ProcessOrder(order);
                        return true;
                    }
                    var contactUs = storeData as StoreContactData;
                    if (contactUs != null)
                    {
                        ProcessContactUs(contactUs);
                        return true;
                    }
                    return false;
                }, TimeSpan.FromMinutes(1), int.MaxValue);
            }
        }

        private void ProcessContactUs(StoreContactData contactUs)
        {
            m_MailComponent.GenerateAndSendEmail("yuval@bizpoint.co.il",
                new StoreContactUs(contactUs.Name, contactUs.Phone, contactUs.University, contactUs.Email,
                    contactUs.Text));
        }

        private void ProcessOrder(StoreOrderData order)
        {
            var productDetail = m_ZboxReadService.GetProductCheckOut(new ViewModel.Queries.GetStoreProductQuery(order.ProdcutId)).Result;

            var features = new KeyValuePair<string, string>[6];
            int index = 0;
            //order.Features
            if (productDetail.Features != null)
                foreach (var feature in productDetail.Features.Where(w => order.Features != null && order.Features.Contains(w.Id)))
                {
                    features[index] = new KeyValuePair<string, string>(feature.Category,
                        feature.Description + "*" + feature.Price + "*");
                    index++;
                }
            for (int i = index; i < 6; i++)
            {
                features[i] = new KeyValuePair<string, string>(string.Empty, string.Empty);
            }


            var retVal = m_WriteService.InsertOrder(new OrderSubmitDto(
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
                  order.NumberOfPayment)).Result;

            m_MailComponent.GenerateAndSendEmail(order.Email,
                new StoreOrder(order.FirstName + " " + order.LastName, retVal.ProductName, retVal.OrderId));
        }


        public void Stop()
        {
            m_KeepRunning = false;
        }


    }
}
