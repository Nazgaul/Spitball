﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.DTOs.Admin
{
    public class DailyStudyRoomsDto
    {
        public DateTime Day { get; set; }
        public int Sessions { get; set; }
        public int Tutors { get; set; }
        public int Users { get; set; }
    }
}
