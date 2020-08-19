using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cloudents.FunctionsV2.Models
{
    public class UpdateEmail
    {
        private int _documentCountUpdate;

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("numUpdates")]
        public int TotalUpdates => DocumentCountUpdate.GetValueOrDefault();

        [JsonProperty("oneUpdate")] public bool OneUpdate => TotalUpdates == 1;

        [JsonProperty("xNewItems")]
        public int? DocumentCountUpdate
        {
            get => _documentCountUpdate == 0 ? (int?)null : _documentCountUpdate;
            set => _documentCountUpdate = value.GetValueOrDefault();
        }

        [JsonProperty("oneItem")] public bool OneItem => DocumentCountUpdate == 1;

        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("courseUpdates")]
        public IEnumerable<Course> Courses { get; set; }

        [JsonProperty("direction")]
        public string Direction { get; set; }

        public UpdateEmail(string userName, string to, bool isRtl)
        {
            UserName = userName;
            To = to;
            Direction = isRtl ? "rtl" : "ltr";
        }

    }
}