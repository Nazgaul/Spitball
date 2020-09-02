﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Event;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "nhibernate proxy")]
    public class Lead : Entity<Guid>
    {
        public Lead( string? text, string referer,
            User user, Tutor? tutor, string utmSource)
        {
            Text = text;
            User = user;
            Referer = referer;
            Tutor = tutor;
            UtmSource = utmSource;
            CreationTime = DateTime.UtcNow;
            AddEvent(new LeadEvent(this));
        }


        [SuppressMessage("ReSharper", "CS8618",Justification = "Nhibernate proxy")]
        protected Lead()
        {
        }

        public virtual User User { get; protected set; }

        public virtual string? Text { get; protected set; }
        public virtual string? Referer { get; protected set; }
        public virtual Tutor? Tutor { get; protected set; }

        public virtual string? UtmSource { get; protected set; }

        public virtual DateTime? CreationTime { get; set; }

        protected internal virtual ISet<ChatRoomAdmin> ChatRoomsAdmin { get; set; }
    }
}