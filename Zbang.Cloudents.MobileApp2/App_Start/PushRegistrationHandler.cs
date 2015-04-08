using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Notifications;
using Microsoft.WindowsAzure.Mobile.Service.Security; 

namespace Zbang.Cloudents.MobileApp2
{
    public class PushRegistrationHandler : INotificationHandler
    {
        public async Task Register(ApiServices services, HttpRequestContext context,
        NotificationRegistration registration)
        {
            try
            {
                // Perform a check here for user ID tags, which are not allowed.
                if (!ValidateTags(registration))
                {
                    throw new InvalidOperationException(
                        "You cannot supply a tag that is a user ID.");
                }

                // Get the logged-in user.
                var currentUser = context.Principal as ServiceUser;

               await services.Push.HubClient.DeleteRegistrationAsync(new Microsoft.ServiceBus.Notifications.GcmRegistrationDescription(registration.DeviceId));
                // Add a new tag that is the user ID.
                registration.Tags.Add(currentUser.GetCloudentsUserId().ToString(CultureInfo.InvariantCulture));

                services.Log.Info("Registered tag for userId: " + currentUser.GetCloudentsUserId());
            }
            catch (Exception ex)
            {
                services.Log.Error(ex.ToString());
            }
            
        }

        private bool ValidateTags(NotificationRegistration registration)
        {
            // Create a regex to search for disallowed tags.
            var searchTerm =
            new System.Text.RegularExpressions.Regex(@"facebook:|google:|twitter:|microsoftaccount:",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            foreach (string tag in registration.Tags)
            {
                if (searchTerm.IsMatch(tag))
                {
                    return false;
                }
            }
            return true;
        }

        public Task Unregister(ApiServices services, HttpRequestContext context,
            string deviceId)
        {
            services.Log.Info("deviceid: " + deviceId + "logged out");
            // This is where you can hook into registration deletion.
            return Task.FromResult(true);
        }
    }
}