﻿using Cloudents.Core.Message;
using Cloudents.Core.Message.Email;
using Cloudents.Core.Message.System;
using Cloudents.Core.Storage;
using Cloudents.Core.Storage.Dto;
using JetBrains.Annotations;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Storage
{
    [UsedImplicitly]
    public class QueueProvider : IQueueProvider
    {
        private readonly CloudQueueClient _queueClient;

        public QueueProvider(ICloudStorageProvider storageProvider)
        {
            _queueClient = storageProvider.GetQueueClient();

        }

        /// <summary>
        /// Insert new question from admin to queue-  this is different because of the nature of the pool
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task InsertMessageAsync(NewQuestionMessage obj, CancellationToken token)
        {
            return InsertMessageAsync(obj, QueueName.QuestionQueue, token);

        }

        public Task InsertMessageAsync(BaseEmail obj, CancellationToken token)
        {
            return InsertMessageAsync(obj, TimeSpan.Zero, token);
        }

        public Task InsertMessageAsync(BaseEmail obj, TimeSpan delay, CancellationToken token)
        {
            var queue = _queueClient.GetQueueReference(QueueName.EmailQueue.Name);
            var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            var cloudMessage = new CloudQueueMessage(json);
            return queue.AddMessageAsync(cloudMessage, null, delay, new QueueRequestOptions(),
                new OperationContext(), token);
        }



        public Task InsertMessageAsync(SmsMessage2 message, CancellationToken token)
        {

            return InsertMessageAsync(message, QueueName.SmsQueue, token);
        }

        public Task InsertMessageAsync(ISystemQueueMessage obj, CancellationToken token)
        {
            var queue = _queueClient.GetQueueReference(QueueName.BackgroundQueue.Name);
            var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            var cloudMessage = new CloudQueueMessage(json);
            token.ThrowIfCancellationRequested();
            return queue.AddMessageAsync(cloudMessage, null, null, new QueueRequestOptions(), new OperationContext(), token);
        }


        private Task InsertMessageAsync(object obj, QueueName queueName, CancellationToken token)
        {
            var queue = _queueClient.GetQueueReference(queueName.Name);
            var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            var cloudMessage = new CloudQueueMessage(json);
            token.ThrowIfCancellationRequested();
            return queue.AddMessageAsync(cloudMessage);
        }
    }
}