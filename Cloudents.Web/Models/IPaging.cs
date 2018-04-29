namespace Cloudents.Web.Models
{
    /// <summary>
    /// Paging interface
    /// </summary>
    public interface IPaging
    {
        /// <summary>
        /// Page property
        /// </summary>
        int? Page { get; set; }
    }
}
