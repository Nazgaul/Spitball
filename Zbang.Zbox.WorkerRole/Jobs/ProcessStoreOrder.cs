﻿using System;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Azure;
using Zbang.Zbox.Infrastructure.Azure.Queue;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Mail.EmailParameters;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;
using Zbang.Zbox.Store.Dto;
using Zbang.Zbox.Store.Services;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.Infrastructure.Trace;

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
            IQueueProviderExtract queueProvider,
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
                ExecuteAsync().Wait();
            }
        }

        private async Task ExecuteAsync()
        {
            await m_QueueProcess.RunQueue(new OrderQueueName(), msg =>
             {
                 try
                 {
                     var storeData = msg.FromMessageProto<StoreData>();
                     var order = storeData as StoreOrderData;
                     if (order != null)
                     {
                         ProcessOrder(order);
                         return Task.FromResult(true);
                     }
                     var contactUs = storeData as StoreContactData;
                     if (contactUs != null)
                     {
                         ProcessContactUs(contactUs);
                         return Task.FromResult(true);
                     }
                     return Task.FromResult(true);
                 }
                 catch (SqlException ex)
                 {
                     m_MailComponent.GenerateAndSendEmail(new[] { "ram@cloudents.com", "eidan@cloudents.com" },
                     "failed connect to remove db " + ex);
                     return Task.FromResult(false);
                 }
             }, TimeSpan.FromMinutes(1), int.MaxValue);
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
            float totalFeaturePrice = 0;
            //order.Features
            if (productDetail.Features != null)
                foreach (var feature in productDetail.Features.Where(w => order.Features != null && order.Features.Contains(w.Id)))
                {
                    features[index] = new KeyValuePair<string, string>(feature.Category,
                        feature.Description + "*" + feature.Price + "*");
                    index++;
                    totalFeaturePrice += feature.Price.HasValue ? feature.Price.Value : 0;
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
                  totalFeaturePrice,
                  order.NumberOfPayment));
            try
            {
                m_MailComponent.GenerateAndSendEmail(order.Email,
                    new StoreOrder(order.FirstName + " " + order.LastName, retVal.ProductName, retVal.OrderId));
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("On sending email store order orderid: " + retVal.OrderId + " email: " + order.Email, ex);
            }
        }


        public void Stop()
        {
            m_KeepRunning = false;
        }


    }
}
