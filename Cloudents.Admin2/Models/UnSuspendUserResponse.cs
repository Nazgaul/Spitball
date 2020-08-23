using System.Collections.Generic;

namespace Cloudents.Admin2.Models
{
    public class UnSuspendUserResponse
    {
        /// <summary>
        /// The User Email
        /// </summary>
        public IEnumerable<string> Email { get; set; }
    }
}