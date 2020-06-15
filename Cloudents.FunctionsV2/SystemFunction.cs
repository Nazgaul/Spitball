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
using Cloudents.Core.Interfaces;
using Cloudents.FunctionsV2.Operations;
using Cloudents.Infrastructure;
using NHibernate;
using NHibernate.Linq;
using SendGrid.Helpers.Mail;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Cloudents.FunctionsV2
{
    public static class SystemFunction
    {



        //[FunctionName("SystemFunction")]
        //public static async Task Run([QueueTrigger(QueueName.BackgroundQueueName)] string queueMsg,
        //    [Inject] ILifetimeScope lifetimeScope,
        //    IBinder binder,
        //    ILogger log,
        //    CancellationToken token)
        //{
        //    //log.LogInformation($"Got message {queueMsg}");

        //    var settings = new JsonSerializerSettings()
        //    {
        //        TypeNameHandling = TypeNameHandling.All,
        //        //Converters.Add(new EnumerationConverter<Country>());
        //    };
        //    settings.Converters.Add(new EnumerationConverter<Country>());

        //    var message = JsonConvert.DeserializeObject<ISystemQueueMessage>(queueMsg, settings);

        //    var handlerType = typeof(ISystemOperation<>).MakeGenericType(message.GetType());
        //    var handlerCollectionType = typeof(IEnumerable<>).MakeGenericType(handlerType);

        //    using var child = lifetimeScope.BeginLifetimeScope();
        //    if (child.Resolve(handlerCollectionType) is IEnumerable handlersCollection)
        //    {
        //        foreach (var handler in handlersCollection)
        //        {
        //            try
        //            {
        //                dynamic operation = child.Resolve(handler.GetType());
        //                await operation.DoOperationAsync((dynamic)message, binder, token);
        //            }
        //            catch (Exception e)
        //            {
        //                log.LogInformation(e.Message);
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// Send an email to update twilio dictionary if new country show up, run every sunday
        ///// </summary>
        ///// <param name="myTimer"></param>
        ///// <param name="session"></param>
        ///// <param name="countryProvider"></param>
        ///// <param name="messageCollector"></param>
        ///// <param name="token"></param>
        ///// <returns></returns>
        //[FunctionName("Email-tutor-newCountries")]
        //public static async Task RunEmailNewCountriesAsync([TimerTrigger("0 0 0 * * 0")] TimerInfo myTimer,
        //    [Inject] IStatelessSession session,
        //    [Inject] ICountryProvider countryProvider,
        //    [SendGrid(ApiKey = "SendgridKey")] IAsyncCollector<SendGridMessage> messageCollector,
        //    CancellationToken token
        //)
        //{
        //    var listOfCountries = await session.Query<Tutor>().Fetch(f => f.User)
        //        .Select(s => s.User.Country).Distinct().ToListAsync(token);

        //    var notSupportedCountry = listOfCountries
        //        .Except(TwilioProvider.CountryToRegionMap.Select(s => s.Key))
        //        .Select(countryProvider.GetCountryParams)
        //        .ToList();
        //    if (notSupportedCountry.Count > 0)
        //    {

        //        var message = new SendGridMessage();
        //        message.AddTo("ram@cloudents.com");
        //        message.AddContent("text/html", notSupportedCountry.ToHtmlTable());
        //        message.SetFrom(new EmailAddress("support@cloudents.com"));
        //        message.SetSubject("not supported countries in twilio");

        //        await messageCollector.AddAsync(message, token);
        //    }
        //}



    }


    public static class HtmlTable
    {
        public static string ToHtmlTable<T>(this List<T> listOfClassObjects)
        {
            var ret = string.Empty;

            return listOfClassObjects == null || !listOfClassObjects.Any()
                ? ret
                : "<table>" +
                  listOfClassObjects.First().GetType().GetFields().Select(p => p.Name).ToList().ToColumnHeaders() +
                  listOfClassObjects.Aggregate(ret, (current, t) => current + t.ToHtmlTableRow()) +
                  "</table>";
        }

        private static string ToColumnHeaders<T>(this List<T> listOfProperties)
        {
            var ret = string.Empty;

            return listOfProperties == null || !listOfProperties.Any()
                ? ret
                : "<tr>" +
                  listOfProperties.Aggregate(ret,
                      (current, propValue) =>
                          current +
                          ("<th style='font-size: 11pt; font-weight: bold; border: 1pt solid black'>" +
                           (Convert.ToString(propValue).Length <= 100
                               ? Convert.ToString(propValue)
                               : Convert.ToString(propValue).Substring(0, 100)) + "..." + "</th>")) +
                  "</tr>";
        }

        private static string ToHtmlTableRow<T>(this T classObject)
        {
            var ret = string.Empty;

            return classObject == null
                ? ret
                : "<tr>" +
                  classObject.GetType()
                      .GetFields()
                      .Aggregate(ret,
                          (current, prop) =>
                              current + ("<td style='font-size: 11pt; font-weight: normal; border: 1pt solid black'>" +
                                         (Convert.ToString(prop.GetValue(classObject)).Length <= 100
                                             ? Convert.ToString(prop.GetValue(classObject))
                                             : Convert.ToString(prop.GetValue(classObject)).Substring(0, 100) +
                                               "...") +
                                         "</td>")) + "</tr>";
        }
    }


}
