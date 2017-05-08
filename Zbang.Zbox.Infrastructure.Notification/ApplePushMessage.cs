using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Zbang.Zbox.Infrastructure.Notifications
{
    [Serializable]
    class ApplePushMessage : Dictionary<string, object>, IPushMessage
    {
        //private const string ApsKey = "aps";
        private static readonly JsonSerializerSettings SerializerSettings;
        /// <summary>
        /// Gets the <see cref="T:Microsoft.WindowsAzure.Mobile.Service.ApsProperties" /> for this <see cref="T:Microsoft.WindowsAzure.Mobile.Service.ApplePushMessage" />.
        /// </summary>
        public ApsProperties Aps => this.GetValueOrDefault<ApsProperties>("aps");

        /// <summary>
        /// Sets or gets the lifetime of the notification. At the end of the lifetime, the notification is no 
        /// longer valid and can be discarded. If this value is non-null, APNs stores the notification and tries 
        /// to deliver the notification at least once. Specify null to indicate that the notification expires 
        /// immediately and that APNs should not store the notification at all.
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset? Expiration
        {
            get;
            set;
        }
        /// <summary>
        /// As an alternative to building the notification by initializing the <see cref="T:Microsoft.WindowsAzure.Mobile.Service.ApplePushMessage" /> directly, 
        /// it is possible to provide a complete JSON representation which will be sent to the Notification Hub unaltered.
        /// </summary>
        public string JsonPayload
        {
            get;
            set;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.WindowsAzure.Mobile.Service.ApplePushMessage" /> class enabling creation a notification
        /// message targeting Apple Push Notification Service. Set the appropriate properties on the message
        /// and submit through the <see cref="T:Microsoft.WindowsAzure.Mobile.Service.Notifications.PushClient" />.
        /// </summary>
        public ApplePushMessage()
            : base(StringComparer.OrdinalIgnoreCase)
        {
            base["aps"] = new ApsProperties();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.WindowsAzure.Mobile.Service.ApplePushMessage" /> class with a given <paramref name="alert" />
        /// message and an optional <paramref name="expiration" /> of the notification. 
        /// </summary>
        /// <param name="alert">The notification alert message.</param>
        /// <param name="expiration">A <see cref="T:System.TimeSpan" /> relative to the current time. At the end of the lifetime, 
        /// the notification is no longer valid and can be discarded. If this value is non-null, APNs stores the 
        /// notification and tries to deliver the notification at least once. Specify null to indicate that the 
        /// notification expires immediately and that APNs should not store the notification at all.</param>
        public ApplePushMessage(string alert, TimeSpan? expiration)
            : this()
        {
            if (alert == null)
            {
                throw new ArgumentNullException(nameof(alert));
            }
            if (expiration.HasValue)
            {
                if (expiration.Value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(nameof(expiration));
                }
                Expiration = DateTimeOffset.UtcNow + expiration.Value;
            }
            Aps.Alert = alert;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.WindowsAzure.Mobile.Service.ApplePushMessage" /> class with the specified serialization information and streaming context.
        /// </summary>
        /// <param name="info">A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> containing information about the <see cref="T:Microsoft.WindowsAzure.Mobile.Service.AlertProperties" /> to be initialized.</param>
        /// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> that indicates the source destination and context information of a serialized stream.</param>
        protected ApplePushMessage(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        /// <summary>
        /// Provides a JSON encoded representation of this <see cref="T:Microsoft.WindowsAzure.Mobile.Service.ApplePushMessage" />
        /// </summary>
        /// <returns>A JSON encoded string.</returns>
        public override string ToString()
        {
            if (JsonPayload != null)
            {
                return JsonPayload;
            }
            return JsonConvert.SerializeObject(this, SerializerSettings);
        }
        static ApplePushMessage()
        {
            // Note: this type is marked as 'beforefieldinit'.
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
            //jsonSerializerSettings.set_Formatting(1);
            SerializerSettings = jsonSerializerSettings;
        }
    }
}
