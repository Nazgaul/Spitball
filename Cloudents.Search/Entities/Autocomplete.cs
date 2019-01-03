using Cloudents.Core.Enum;
using Cloudents.Search.Interfaces;

namespace Cloudents.Search.Entities
{
    public class AutoComplete : ISearchObject
    {
        public string Id { get; set; }

        /// <summary>
        /// The auto suggest list for the user
        /// </summary>
        public string Key { get; set; }

        public string Prefix { get; set; }

        /// <summary>
        /// The End result to search given a key
        /// </summary>
        public string Value { get; set; }

        public Vertical Vertical { get; set; }
    }
}