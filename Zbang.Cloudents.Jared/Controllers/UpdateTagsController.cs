using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using Newtonsoft.Json.Linq;
using Zbang.Cloudents.Jared.Models;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController]
    public class UpdateTagsController : ApiController
    {
        private NotificationHubClient m_HubClient;

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);

            // Get the Mobile App settings.
            var settings =
                Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();

            // Get the Notification Hubs credentials for the Mobile App.
            var notificationHubName = settings.NotificationHubName;
            var notificationHubConnection = settings
                .Connections[MobileAppSettingsKeys.NotificationHubConnectionString]
                .ConnectionString;

            // Create the notification hub client.
            m_HubClient = NotificationHubClient
                .CreateClientFromConnectionString(notificationHubConnection,
                    notificationHubName);
        }
        // GET api/UpdateTags
        [HttpGet]
        public async Task<List<string>> Get(string id)
        {
            // Return the installation for the specific ID.
            var installation = await m_HubClient.GetInstallationAsync(id).ConfigureAwait(false);
            return installation.Tags as List<string>;
        }

        //[HttpDelete]
        //public string Delete(string id)
        //{
        //    return id;
        //}
        [HttpPost]
        public async Task<HttpResponseMessage> Post(string id)
        {
            // Get the tags to update from the body of the request.
            var message = await Request.Content.ReadAsStringAsync().ConfigureAwait(false);

            // Validate the submitted tags.
            if (string.IsNullOrEmpty(message) || message.Contains("sid:"))
            {
                // We can't trust users to submit their own user IDs.
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            // Verify that the tags are a valid JSON array.
            var tags = JArray.Parse(message);

            // Define a collection of PartialUpdateOperations. Note that 
            // only one '/tags' path is permitted in a given collection.
            var updates = new List<PartialUpdateOperation>
            {
                new PartialUpdateOperation
                {
                    Operation = UpdateOperationType.Add,
                    Path = "/tags",
                    Value = tags.ToString()
                }
            };
            // Add a update operation for the tag.
            try
            {
                // Add the requested tag to the installation.
                await m_HubClient.PatchInstallationAsync(id, updates).ConfigureAwait(false);
                // Return success status.
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (MessagingException)
            {
                // When an error occurs, return a failure status.
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}
