using System;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    //public class FlagQuestionRequest
    //{
    //    /// <summary>
    //    /// Question Id
    //    /// </summary>
    //    public long Id { get; set; }

    //    /// <summary>
    //    /// The flag reason
    //    /// </summary>
    //    [StringLength(255)]
    //    public string FlagReason { get; set; }
    //}

    //public class FlagAnswerRequest
    //{
    //    /// <summary>
    //    /// Answer Id
    //    /// </summary>
    //    public Guid Id { get; set; }

    //    /// <summary>
    //    /// The flag reason
    //    /// </summary>
    //    [StringLength(255)]
    //    public string FlagReason { get; set; }
    //}

    public class FlagDocumentRequest
    {
        /// <summary>
        /// Document Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The flag reason
        /// </summary>
        [StringLength(255)]
        public string FlagReason { get; set; }
    }

    public class PurchaseDocumentRequest
    {
        public long Id { get; set; }
    }


}
