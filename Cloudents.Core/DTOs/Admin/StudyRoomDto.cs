﻿using System;

namespace Cloudents.Core.DTOs.Admin
{
    public class StudyRoomDto
    {
        public string TutorName { get; set; }
        public string UserName { get; set; }
        public DateTime Created { get; set; }
        public int Duration { get; set; }
    }
}
