﻿namespace Cloudents.Web.Models
{
    public class GetQuestionsRequest : IPaging
    {
      //  [DisplayFormat(HtmlEncode = true)]
        public string Term { get; set; }
        public int? Page { get; set; }

        public string[] Source { get; set; }
    }
}