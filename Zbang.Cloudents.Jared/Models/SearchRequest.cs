﻿namespace Zbang.Cloudents.Jared.Models
{
    public class SearchRequest
    {
        public string[] Query { get; set; }
        public int? Page { get; set; }

        public long? University { get; set; }
        public string Course { get; set; }
    }
}