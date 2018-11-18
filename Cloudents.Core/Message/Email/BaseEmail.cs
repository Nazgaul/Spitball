﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using JetBrains.Annotations;

namespace Cloudents.Core.Message.Email
{
    [Serializable]
    public abstract class BaseEmail
    {
        protected BaseEmail(string to, string subject, [CanBeNull] CultureInfo info)
        {
            To = to;
            Subject = subject;
            TemplateId = AssignTemplate(info);
            //Campaign = campaign;
        }

        [DebuggerDisplay("To = {To}")]

        public string To { get; private set; }

        public string TemplateId { get; private set; }
        public string Subject { get; private set; }
        [CanBeNull] public abstract string Campaign { get; }

        //public abstract string TemplateEnglishId { get; }
        //public abstract string TemplateHebrewId { get; }

        [CanBeNull] protected abstract IDictionary<CultureInfo, string> Templates { get;  }

        private string AssignTemplate(CultureInfo info)
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
            if (Templates.TryGetValue(Language.English.Culture, out var template2))
            {
                return template2;
            }
            return null;
        }


    }
}