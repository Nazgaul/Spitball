﻿using System;

namespace Cloudents.Core.DTOs
{
    public class StudyRoomDto
    {
       
        public string OnlineDocument { get; set; }
        public Guid ConversationId { get; set; }
        public string SessionId { get; set; }

        public long TutorId { get; set; }
    }


    public class UserStudyRoomDto
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public bool Online { get; set; }
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
    }
}