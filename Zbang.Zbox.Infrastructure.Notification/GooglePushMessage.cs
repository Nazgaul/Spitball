using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Zbang.Zbox.Infrastructure.Notifications
{
    [Serializable]
    public class GooglePushMessage : Dictionary<string, object>, IPushMessage
    {
        private const string CollapseKeyKey = "collapse_key";
        private const string DelayWhileIdleKey = "delay_while_idle";
        private const string DataKey = "data";
        private const string TimeToLiveKey = "time_to_live";
        private static readonly TimeSpan MaxExpiration = TimeSpan.FromDays(28.0);
        private static readonly JsonSerializerSettings SerializerSettings;
        /// <summary>
        /// A collapse key is an arbitrary string that is used to collapse a group of like messages when the device 
        /// is offline, so that only the most recent message gets sent to the client. For example, "New mail", "Updates available", 
        /// and so on.
        /// </summary>
        public string CollapseKey
        {
            get
            {
                return this.GetValueOrDefault("collapse_key");
            }
            set
            {
                this.SetOrClearValue("collapse_key", value);
            }
        }
        /// <summary>
        /// Indicates whether the message should be delivered while the device is idle.
        /// </summary>
        public bool DelayWhileIdle
        {
            get
            {
                return this.GetValueOrDefault("delay_while_idle");
            }
            set
            {
                this.SetOrClearValue("delay_while_idle", value);
            }
        }
        /// <summary>
        /// A collection or name-value properties to include in the message. Properties must be simple types, i.e. they 
        /// can not be nested.
        /// </summary>
        public IDictionary<string, string> Data
        {
            get
            {
                return this.GetValueOrDefault("data");
            }
        }
        /// <summary>
        /// The Time to Live (TTL) property lets the sender specify the maximum lifespan of a message. The value of this 
        /// parameter must be a duration from 0 to 2,419,200 seconds, and it corresponds to the maximum period of time 
        /// for which GCM will store and try to deliver the message. Requests that don't contain this field default 
        /// to the maximum period of 4 weeks.
        /// </summary>
        public int? TimeToLiveInSeconds
        {
            get
            {
                return this.GetValueOrDefault("time_to_live");
            }
            set
            {
                this.SetOrClearValue("time_to_live", value);
            }
        }
        /// <summary>
        /// As an alternative to building the notification by initializing the <see cref="T:Microsoft.WindowsAzure.Mobile.Service.GooglePushMessage" /> directly, 
        /// it is possible to provide a complete JSON representation which will be sent to the Notification Hub unaltered.
        /// </summary>
        public string JsonPayload
        {
            get;
            set;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.WindowsAzure.Mobile.Service.GooglePushMessage" /> class enabling creation
        /// of a notification message targeting Google Cloud Messaging for Chrome (GCM).Set the 
        /// appropriate properties on the message and submit through the <see cref="T:Microsoft.WindowsAzure.Mobile.Service.Notifications.PushClient" />
        /// </summary>
        public GooglePushMessage()
            : base(StringComparer.OrdinalIgnoreCase)
        {
            base["data"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.WindowsAzure.Mobile.Service.GooglePushMessage" /> class with a given
        /// set of <paramref name="data" /> parameters and an optional <paramref name="timeToLive" />.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="timeToLive">A <see cref="T:System.TimeSpan" /> relative to the current time. The value of this 
        /// parameter must be a duration from 0 to 2,419,200 seconds (28 days), and it corresponds to the maximum period of time 
        /// for which GCM will store and try to deliver the message. Requests that don't contain this field default 
        /// to the maximum period of 4 weeks.</param>
        public GooglePushMessage(IDictionary<string, string> data, TimeSpan? timeToLive)
            : this()
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (timeToLive.HasValue)
            {
                if (timeToLive.Value < TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException("timeToLive", timeToLive.Value, CommonResources.ArgumentOutOfRange_GreaterThan.FormatForUser(new object[]
					{
						TimeSpan.Zero
					}));
                }
                if (timeToLive.Value > GooglePushMessage.MaxExpiration)
                {
                    throw new ArgumentOutOfRangeException("timeToLive", timeToLive.Value, CommonResources.ArgumentOutOfRange_LessThan.FormatForUser(new object[]
					{
						GooglePushMessage.MaxExpiration
					}));
                }
                this.TimeToLiveInSeconds = new int?((int)timeToLive.Value.TotalSeconds);
            }
            foreach (KeyValuePair<string, string> current in data)
            {
                this.Data.Add(current);
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.WindowsAzure.Mobile.Service.GooglePushMessage" /> class with the specified serialization information and streaming context.
        /// </summary>
        /// <param name="info">A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> containing information about the <see cref="T:Microsoft.WindowsAzure.Mobile.Service.GooglePushMessage" /> to be initialized.</param>
        /// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> that indicates the source destination and context information of a serialized stream.</param>
        protected GooglePushMessage(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            base["data"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
        /// <summary>
        /// Provides a JSON encoded representation of this <see cref="T:Microsoft.WindowsAzure.Mobile.Service.GooglePushMessage" />
        /// </summary>
        /// <returns>A JSON encoded string.</returns>
        public override string ToString()
        {
            if (this.JsonPayload != null)
            {
                return this.JsonPayload;
            }
            return JsonConvert.SerializeObject(this, GooglePushMessage.SerializerSettings);
        }
        static GooglePushMessage()
        {
            // Note: this type is marked as 'beforefieldinit'.
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.set_Formatting(1);
            GooglePushMessage.SerializerSettings = jsonSerializerSettings;
        }
    }
}
