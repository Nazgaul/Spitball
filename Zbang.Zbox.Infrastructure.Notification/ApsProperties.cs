using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Zbang.Zbox.Infrastructure.Notifications
{
    [Serializable]
    class ApsProperties : Dictionary<string, object>
    {
        //private const string AlertKey = "alert";
        //private const string BadgeKey = "badge";
        //private const string SoundKey = "sound";
        //private const string ContentAvailableKey = "content-available";
        /// <summary>
        /// The alert message as a single string. For more complex alert message options, please use <see cref="M:AlertProperties" />.
        /// </summary>
        public string Alert
        {
            get
            {
                return this.GetValueOrDefault<string>("alert");
            }
            set
            {
                this.SetOrClearValue("alert", value);
            }
        }
        /// <summary>
        /// The alert message as a dictionary with additional properties describing the alert such as localization information, which image to display, etc.
        /// If the alert is simply a string then please use <see cref="M:Alert" />.
        /// </summary>
        public AlertProperties AlertProperties
        {
            get
            {
                AlertProperties alertProperties = this.GetValueOrDefault<AlertProperties>("alert");
                if (alertProperties == null)
                {
                    alertProperties = new AlertProperties();
                    base["alert"] = alertProperties;
                }
                return alertProperties;
            }
        }
        /// <summary>
        /// The number to display as the badge of the application icon. If this property is absent, the badge is not changed. 
        /// To remove the badge, set the value of this property to 0.
        /// </summary>
        public int? Badge
        {
            get
            {
                return this.GetValueOrDefault<int?>("badge");
            }
            set
            {
                this.SetOrClearValue("badge", value);
            }
        }
        /// <summary>
        /// The name of a sound file in the application bundle. The sound in this file is played as an alert. If the sound file 
        /// doesn’t exist or default is specified as the value, the default alert sound is played. The audio must be in one 
        /// of the audio data formats that are compatible with system sounds; 
        /// </summary>
        public string Sound
        {
            get
            {
                return this.GetValueOrDefault<string>("sound");
            }
            set
            {
                this.SetOrClearValue("sound", value);
            }
        }
        /// <summary>
        /// Provide this key with a value of 1 to indicate that new content is available. This is used to support 
        /// Newsstand apps and background content downloads.
        /// </summary>
        public bool ContentAvailable
        {
            get
            {
                return this.GetValueOrDefault<bool>("content-available");
            }
            set
            {
                this.SetOrClearValue("content-available", value);
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.WindowsAzure.Mobile.Service.ApsProperties" /> class.
        /// </summary>
        public ApsProperties()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.WindowsAzure.Mobile.Service.ApsProperties" /> class with the specified serialization information and streaming context.
        /// </summary>
        /// <param name="info">A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> containing information about the <see cref="T:Microsoft.WindowsAzure.Mobile.Service.ApsProperties" /> to be initialized.</param>
        /// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> that indicates the source destination and context information of a serialized stream.</param>
        protected ApsProperties(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
