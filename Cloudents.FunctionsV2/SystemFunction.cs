using Autofac;
using Cloudents.Core.Message.System;
using Cloudents.Core.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.FunctionsV2.Operations;
using Cloudents.Infrastructure;
using NHibernate;
using NHibernate.Linq;
using SendGrid.Helpers.Mail;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace Cloudents.FunctionsV2
{
    public static class SystemFunction
    {

       

        [FunctionName("SystemFunction")]
        public static async Task Run([QueueTrigger(QueueName.BackgroundQueueName)]string queueMsg,
            [Inject] ILifetimeScope lifetimeScope,
            IBinder binder,
            ILogger log,
            CancellationToken token)
        {
            //log.LogInformation($"Got message {queueMsg}");

            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                //Converters.Add(new EnumerationConverter<Country>());
            };
            settings.Converters.Add(new EnumerationConverter<Country>());

            var message = JsonConvert.DeserializeObject<ISystemQueueMessage>(queueMsg, settings);

            var handlerType = typeof(ISystemOperation<>).MakeGenericType(message.GetType());
            var handlerCollectionType = typeof(IEnumerable<>).MakeGenericType(handlerType);

            using var child = lifetimeScope.BeginLifetimeScope();
            if (child.Resolve(handlerCollectionType) is IEnumerable handlersCollection)
            {
                foreach (var handler in handlersCollection)
                {
                    try
                    {
                        dynamic operation = child.Resolve(handler.GetType());
                        await operation.DoOperationAsync((dynamic)message, binder, token);
                    }
                    catch (Exception e)
                    {
                        log.LogInformation(e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Send an email to update twilio dictionary if new country show up, run every sunday
        /// </summary>
        /// <param name="myTimer"></param>
        /// <param name="session"></param>
        /// <param name="messageCollector"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [FunctionName("Email-tutor-newCountries")]
        public static async Task RunEmailNewCountriesAsync([TimerTrigger("0 0 0 * * 0")] TimerInfo myTimer,
            [Inject] IStatelessSession session,
            [SendGrid(ApiKey = "SendgridKey")] IAsyncCollector<SendGridMessage> messageCollector,
            CancellationToken token
        )
        {
            var listOfCountries = await session.Query<Tutor>().Fetch(f => f.User)
                .Select(s => s.User.Country).Distinct().ToListAsync(token);

            var notSupportedCountry = listOfCountries.Except(TwilioProvider.CountryToRegionMap.Select(s => s.Key)).ToList();
            if (notSupportedCountry.Count > 0)
            {
                var message = new SendGridMessage();
                message.AddTo("ram@cloudents.com");
                message.AddContent("text/html", string.Join("<br>", notSupportedCountry));
                message.SetFrom(new EmailAddress("suuport@cloudents.com"));
                message.SetSubject("not supported countries in twilio");

                await messageCollector.AddAsync(message, token);
            }
        }
    }
}
