using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Notifications
{
    [Serializable]
    class AlertProperties : Dictionary<string, object>
    {
        //private const string BodyKey = "body";
        //private const string ActionLocKeyKey = "action-loc-key";
        //private const string LocKeyKey = "loc-key";
        //private const string LocArgsKey = "loc-args";
        //private const string LaunchImageKey = "launch-image";
        /// <summary>
        /// The text of the alert message.
        /// </summary>
        public string Body
        {
            get
            {
                return this.GetValueOrDefault<string>("body");
            }
            set
            {
                this.SetOrClearValue("body", value);
            }
        }

        public string Title
        {
            get
            {
                return this.GetValueOrDefault<string>("title");
            }
            set
            {
                this.SetOrClearValue("title", value);
            }
        }
        /// <summary>
        /// If a string is specified, the system displays an alert with two buttons. The string is used as a key to get a localized string in the current localization to use for the right button’s title instead of “View”.
        /// </summary>
        public string ActionLocKey
        {
            get
            {
                return this.GetValueOrDefault<string>("action-loc-key");
            }
            set
            {
                base["action-loc-key"] = value;
            }
        }
        /// <summary>
        /// A key to an alert-message string in a <c>Localizable.strings</c> file for the current localization (which is set by the user’s language preference). The key string can be formatted with <c>%@</c> and <c>%n$@</c> specifiers to take the variables specified in <c>loc-args</c>. 
        /// </summary>
        public string LocKey
        {
            get
            {
                return this.GetValueOrDefault<string>("loc-key");
            }
            set
            {
                this.SetOrClearValue("loc-key", value);
            }
        }
        /// <summary>
        /// Variable string values to appear in place of the format specifiers in <c>loc-key</c>.
        /// </summary>
        public Collection<string> LogArgs
        {
            get
            {
                Collection<string> collection = this.GetValueOrDefault<Collection<string>>("loc-args");
                if (collection == null)
                {
                    collection = new Collection<string>();
                    base["loc-args"] = collection;
                }
                return collection;
            }
        }
        /// <summary>
        /// The filename of an image file in the application bundle; it may include the extension or omit it. The image is used as the launch image 
        /// when users tap the action button or move the action slider. If this property is not specified, the system either uses the previous 
        /// snapshot, uses the image identified by the UILaunchImageFile key in the application’s <c>Info.plist</c> file, or falls back to <c>Default.png</c>.
        /// </summary>
        public string LaunchImage
        {
            get
            {
                return this.GetValueOrDefault<string>("launch-image");
            }
            set
            {
                this.SetOrClearValue("launch-image", value);
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.WindowsAzure.Mobile.Service.AlertProperties" /> class.
        /// </summary>
        public AlertProperties()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.WindowsAzure.Mobile.Service.AlertProperties" /> class with the specified serialization information and streaming context.
        /// </summary>
        /// <param name="info">A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> containing information about the <see cref="T:Microsoft.WindowsAzure.Mobile.Service.AlertProperties" /> to be initialized.</param>
        /// <param name="context">A <see cref="T:System.Runtime.Serialization.StreamingContext" /> that indicates the source destination and context information of a serialized stream.</param>
        protected AlertProperties(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
