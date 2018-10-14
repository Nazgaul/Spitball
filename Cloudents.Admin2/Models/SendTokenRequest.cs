using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Cloudents.Core.Attributes;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;

namespace Cloudents.Admin2.Models
{
    public class SendTokenRequest : IValidatableObject
    {
        /// <summary>
        /// User id
        /// </summary>
        [Required]
        public long UserId { get; set; }
        /// <summary>
        /// Amount of token that are earned
        /// </summary>
        [Required]
        public decimal Tokens { get; set; }

        /// <summary>
        /// Which token - awarded or earned
        /// </summary>
        [Required]
        public TransactionType TransactionType { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var t = TransactionType.GetAttributeValue<PublicValueAttribute>();
            if (t == null)
            {
                yield return new ValidationResult(
                    "Not valid transaction type",
                    new[] { nameof(TransactionType) });
            }
        }
    }
}
