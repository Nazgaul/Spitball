using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Notifications;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Cloudents.MobileApp2
{
    public class PushRegistrationHandler : INotificationHandler
    {
        public IZboxWriteService ZboxWriteService { get; set; }

        public  Task Register(ApiServices services, HttpRequestContext context,
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
                var userId = currentUser.GetCloudentsUserId();
                //wns, mpns, apns, or gcm
                var tagId = userId.ToString(CultureInfo.InvariantCulture);
                try
                {
                    var os = MobileOperatingSystem.None;
                    if (registration.Platform == "apns")
                    {
                        os = MobileOperatingSystem.iOS;
                    }
                    if (registration.Platform == "gcm")
                    {
                        os = MobileOperatingSystem.Android;
                    }
                    if (os != MobileOperatingSystem.None)
                    {
                        var command = new RegisterMobileDeviceCommand(userId, os);
                        ZboxWriteService.RegisterMobileDevice(command);
                    }
                }
                catch (Exception ex)
                {
                    services.Log.Error(ex.ToString());
                }
             
                
                registration.Tags.Add(tagId);
                if (registration.Tags.Count > 1)
                {
                    services.Log.Warn("Got more tags " + String.Join(" ", registration.Tags));
                }
            }
            catch (Exception ex)
            {
                services.Log.Error(ex.ToString());
            }
            return Task.FromResult(true);

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
            //var currentUser = context.Principal as ServiceUser;

            //services.Log.Info(string.Format("deviceid: {0} of user {1} logged out", deviceId, currentUser.GetCloudentsUserId()));
            // This is where you can hook into registration deletion.
            return Task.FromResult(true);
        }
    }
}