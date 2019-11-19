using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;

namespace Cloudents.FunctionsV2.Binders
{
    internal class TwilioCallMessageAsyncCollector : IAsyncCollector<CreateCallOptions>
    {
        private readonly TwilioRestClient _context;
        private readonly ConcurrentQueue<CreateCallOptions> _messages = new ConcurrentQueue<CreateCallOptions>();

        public TwilioCallMessageAsyncCollector(TwilioRestClient context)
        {
            _context = context;
        }

        public Task AddAsync(CreateCallOptions messageOptions, CancellationToken cancellationToken = default(CancellationToken))
        {
            //ApplyContextMessageSettings(messageOptions, _context);

            if (messageOptions.To == null)
            {
                throw new InvalidOperationException("A 'To' number must be specified for the message.");
            }

            if (messageOptions.From == null)
            {
                throw new InvalidOperationException("A 'From' number must be specified for the message.");
            }

            if (messageOptions.Url == null)
            //if (messageOptions.Body == null)
            {
                throw new InvalidOperationException("A 'Url' must be specified for the message.");
            }

            _messages.Enqueue(messageOptions);

            return Task.CompletedTask;
        }

        //internal static void ApplyContextMessageSettings(CreateCallOptions messageOptions, TwilioSmsContext context)
        //{

        //    if (messageOptions.From == null)
        //    {
        //        messageOptions.From = new PhoneNumber(context.From);
        //    }

        //    //if (messageOptions.Body == null)
        //    //{
        //    //    messageOptions.Body = context.Body;
        //    //}
        //}

        public async Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            while (_messages.TryDequeue(out CreateCallOptions message))
            {
                // this create will initiate the send operation
                await CallResource.CreateAsync(message, client: _context);
            }
        }
    }
}