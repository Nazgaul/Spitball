﻿namespace Zbang.Cloudents.Jared.Models
{
    public class AskRequest
    {
        public string UserText { get; set; }
        public int? Page { get; set; }
        public string[] Term { get; set; }
    }
}