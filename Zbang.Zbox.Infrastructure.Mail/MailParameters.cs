using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Infrastructure.Mail
{
    public abstract class MailParameters
    {
        internal const string WelcomeResolver = "Welcome";
        internal const string InvitationToCloudentsResolver = "InviteCloudents";
        internal const string ForgotPswResolver = "Forgot";
        internal const string InviteResolver = "Invite";
        internal const string MessageResolver = "Message";
        internal const string UpdateResolver = "Update";
        internal const string ChangeEmailResolver = "ChangeEmail";
        internal const string FlagBadItemResolver = "FlagBadItem";

        internal const string DefaultEmail = "no-reply@cloudents.com";
        internal const string DefaultSenderName = "Cloudents";
        public MailParameters(CultureInfo culture)
            : this(culture, DefaultEmail, DefaultSenderName)
        {

        }
        public MailParameters(CultureInfo culture, string senderEmail, string senderName)
        {
            UserCulture = culture;
            SenderEmail = senderEmail;
            SenderName = senderName;
        }

        public abstract string MailResover { get; }
        public CultureInfo UserCulture { get; private set; }

        public string SenderEmail { get; private set; }
        public string SenderName { get; private set; }
    }

    public class WelcomeMailParams : MailParameters
    {
        public WelcomeMailParams(string name, CultureInfo culture)
            : base(culture)
        {
            Name = name;
        }
        public string Name { get; private set; }

        public override string MailResover
        {
            get { return WelcomeResolver; }
        }
    }
    public class InvitationToCloudentsMailParams : MailParameters
    {
        public InvitationToCloudentsMailParams(string senderName, string senderImage, CultureInfo culture)
            : base(culture)
        {
            SenderName = senderName;
            SenderImage = senderImage;
        }
        public string SenderName { get; private set; }
        public string SenderImage { get; private set; }

        public override string MailResover
        {
            get { return InvitationToCloudentsResolver; }
        }
    }


    public class ForgotPasswordMailParams2 : MailParameters
    {
        public ForgotPasswordMailParams2(string code, string link, string name, CultureInfo culture)
            : base(culture)
        {
            Code = code;
            Link = link;
            Name = name;
        }
        public string Code { get; private set; }
        public string Link { get; private set; }
        public string Name { get; private set; }
        public override string MailResover
        {
            get { return MailParameters.ForgotPswResolver; }
        }
    }

    public class FlagItemMailParams : MailParameters
    {
        public FlagItemMailParams(string itemName, string reason, string userName, string email, string url)
            : base(new CultureInfo("en-Us"))
        {
            ItemName = itemName;
            Reason = reason;
            UserName = userName;
            Email = email;
            Url = url;
        }
        public override string MailResover
        {
            get { return MailParameters.FlagBadItemResolver; }
        }
        public string ItemName { get; private set; }
        public string Reason { get; private set; }
        public string UserName { get; private set; }
        public string Email { get; private set; }

        public string Url { get; private set; }
    }


    public class InviteMailParams : MailParameters
    {
        public InviteMailParams(string invitor, string boxname, string boxurl,string invitorImage, CultureInfo culture)
            : base(culture)
        {
            Invitor = invitor;
            BoxName = boxname;
            BoxUrl = boxurl;
            InvitorImage = invitorImage;
        }
        public string Invitor { get; private set; }
        public string BoxName { get; private set; }
        public string BoxUrl { get; private set; }
        public string InvitorImage { get; set; }
        public override string MailResover
        {
            get { return MailParameters.InviteResolver; }
        }
    }

    public class MessageMailParams : MailParameters
    {
        public MessageMailParams(string message, string senderUserName, CultureInfo culture)
            : base(culture, DefaultEmail, senderUserName)
        {
            Message = message;
            //SenderUserName = senderUserName;
        }
        public MessageMailParams(string message, string senderUserName, CultureInfo culture, string senderEmail, string senderImage)
            : base(culture, senderEmail, senderUserName)
        {
            Message = message;
            SenderImage = senderImage;
        }
        public string Message { get; private set; }
        //public string SenderUserName { get; private set; }
        public string SenderImage { get; private set; }
        //public string SenderEmail { get; private set; }
        public override string MailResover
        {
            get { return MessageResolver; }
        }
    }

    public class UpdateMailParams : MailParameters
    {
        public UpdateMailParams(IEnumerable<BoxUpdate> updates, CultureInfo culture)
            : base(culture)
        {
            Updates = updates;
        }
        public override string MailResover
        {
            get { return UpdateResolver; }
        }

        public IEnumerable<BoxUpdate> Updates { get; private set; }

        public class BoxUpdate
        {
            public BoxUpdate(string boxName, IEnumerable<BoxUpdateDetails> updates)
            {
                BoxName = boxName;
                Updates = updates;
            }
            public string BoxName { get; private set; }
            public IEnumerable<BoxUpdateDetails> Updates { get; private set; }

        }
        public class BoxUpdateDetails
        {
            public BoxUpdateDetails(long userId, string userName, string actionElement, EmailAction actionText, string actionUrl)
            {
                UserId = userId;
                UserName = userName;
                ActionElement = actionElement;
                ActionText = actionText;
                ActionUrl = actionUrl;
            }
            public long UserId { get; private set; }
            public string UserName { get; private set; }
            public string ActionElement { get; private set; }
            public EmailAction ActionText { get; private set; }

            public string ActionUrl { get; private set; }
        }
    }


    public class ChangeEmailMailParams : MailParameters
    {
        public ChangeEmailMailParams(string code, CultureInfo culture)
            : base(culture)
        {
            Code = code;
        }
        public string Code { get; private set; }

        public override string MailResover
        {
            get { return ChangeEmailResolver; }
        }
    }




}
