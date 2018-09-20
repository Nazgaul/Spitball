﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Message;
using Cloudents.Core.Request;

namespace Cloudents.Core.Storage
{
    public interface IServiceBusProvider
    {
        Task InsertMessageAsync(BaseEmail message, CancellationToken token);
        Task InsertMessageAsync(BlockChainInitialBalance message, CancellationToken token);

        Task InsertMessageAsync(BlockChainQnaSubmit message, CancellationToken token);
        Task InsertMessageAsync(UrlRedirectQueueMessage message, CancellationToken token);
        Task InsertMessageAsync(SmsMessage2 message, CancellationToken token);

        Task InsertMessageAsync(ServiceBusMessageBase message, CancellationToken token);
    }


    public interface IDataProtect
    {
        string Protect(string purpose, string plaintext, DateTimeOffset expiration);

        string Unprotect(string purpose, string protectedData);
    }
}