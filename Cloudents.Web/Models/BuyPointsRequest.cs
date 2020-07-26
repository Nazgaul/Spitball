//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using Cloudents.Core;

//namespace Cloudents.Web.Models
//{
//    public sealed class BuyPointsRequest : IValidatableObject
//    {
//        [Required]
//        public int Points { get; set; }


//        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
//        {
//            var val = Enumeration.FromValue<PointBundle>(Points);
//            if (val == null)
//            {
//                yield return new ValidationResult("Invalid amount");
//            }
//            // PointBundle.Parse(Points);
//        }
//    }
//}