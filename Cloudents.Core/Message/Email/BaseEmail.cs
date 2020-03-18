using Cloudents.Core.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cloudents.Core.Message.Email
{
    [Serializable]
    public abstract class BaseEmail
    {
        protected BaseEmail(string to, string subject, CultureInfo? info)
        {
            To = to;
            Subject = subject;
            Info = info;
            TemplateId = AssignTemplate(Info);
        }

        protected BaseEmail()
        {

        }

        public CultureInfo Info { get; set; }

        public string To { get; private set; }

        public virtual string[] Bcc { get; protected set; }

        public string TemplateId { get; protected set; }
        public string Subject { get; protected set; }
        public abstract string? Campaign { get; }

        public abstract UnsubscribeGroup UnsubscribeGroup { get; }
        protected abstract IDictionary<CultureInfo, string>? Templates { get; }

        private string? AssignTemplate(CultureInfo info)
        {
            if (Templates == null)
            {
                return null;
            }
            while (info != null)
            {

                if (Templates.TryGetValue(info, out var template))
                {
                    return template;
                }

                if (Equals(info, info.Parent))
                {
                    break;
                }
                info = info.Parent;
            }
            if (Templates.TryGetValue(Language.English, out var template2))
            {
                return template2;
            }
            return null;
        }
    }

    public sealed class UnsubscribeGroup
    {
        public static readonly UnsubscribeGroup Update = new UnsubscribeGroup(16556);
        public static readonly UnsubscribeGroup System = new UnsubscribeGroup(13505);
        private UnsubscribeGroup(int groupId)
        {
            GroupId = groupId;
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Need for deserialize")]
        private UnsubscribeGroup()
        {

        }

        public int GroupId { get; private set; }

        public static implicit operator int(UnsubscribeGroup group)
        {
            return group.GroupId;
        }
    }

    //public interface IEmailMessage {

    //}
}