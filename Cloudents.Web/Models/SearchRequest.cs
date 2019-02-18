using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Enum;
using Cloudents.Core.Models;
using Cloudents.Web.Binders;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Models
{
    /// <inheritdoc />
    /// <summary>
    /// Document and flashcard search object
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Api model")]
    public class SearchRequest : IPaging
    {
        /// <summary>
        /// User courses id
        /// </summary>
        public string Course { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Page for paging
        /// </summary>
        public int? Page { get; set; }

        /// <summary>
        /// The term array of Ai parse
        /// </summary>
       // [DisplayFormat(HtmlEncode = true)]
        public string Query { get; set; }

        /// <summary>
        /// User sort option
        /// </summary>
        [DefaultValue(0)]
        public SearchRequestSort? Sort { get; set; }

        /// <summary>
        /// Array of sites to search for
        /// </summary>

        [DisplayFormat(HtmlEncode = true)]
        public string[] Source { get; set; }
      
    }

    public class DocumentRequest : VerticalRequest
    {
        public string[] Filter { get; set; }

    }

    public abstract class VerticalRequest : IPaging
    {
        /// <summary>
        /// User courses id
        /// </summary>
        public string Course { get; set; }

        [FromQuery(Name = "Uni")]
        public string University { get; set; }

        [IgnoreNextPageLink]
        public bool NeedUniversity => !string.IsNullOrEmpty(University);
        /// <inheritdoc />
        /// <summary>
        /// Page for paging
        /// </summary>
        public int? Page { get; set; }

        /// <summary>
        /// The term of search
        /// </summary>
        // [DisplayFormat(HtmlEncode = true)]
        public string Term { get; set; }

        [ProfileModelBinder(ProfileServiceQuery.University | ProfileServiceQuery.Country |
                            ProfileServiceQuery.Course | ProfileServiceQuery.Tag)]
        [IgnoreNextPageLink]
        public UserProfile Profile { get; set; }
    }
}