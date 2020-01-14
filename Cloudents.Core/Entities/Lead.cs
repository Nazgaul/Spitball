﻿using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Event;

namespace Cloudents.Core.Entities
{
    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "nhibernate proxy")]
    public class Lead : Entity<Guid>
    {
        public Lead(string course, string text, string referer,
            [CanBeNull] User user, Tutor tutor, string utmSource)
        {
            Course = course;
            Text = text;
            User = user;
            Referer = referer;
            Tutor = tutor;
            UtmSource = utmSource;
            CreationTime = DateTime.UtcNow;

            AddEvent(new LeadEvent(this));
        }


        protected Lead()
        {
        }

        [CanBeNull]
        public virtual User User { get; protected set; }
        public virtual string Course { get; protected set; }

        public virtual string Text { get; protected set; }
        public virtual string Referer { get; protected set; }
        [CanBeNull]
        public virtual Tutor Tutor { get; protected set; }

        public virtual string UtmSource { get; protected set; }

        public virtual DateTime? CreationTime { get; set; }
  

        //private readonly ISet<ChatRoomAdmin> _chatRoomsAdmin = new HashSet<ChatRoomAdmin>();
        protected internal virtual ISet<ChatRoomAdmin> ChatRoomsAdmin { get; set; }
    }
}