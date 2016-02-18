using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Cobisi.EmailVerify;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    public class ValidateEmailAttribute : ValidationAttribute
    {
        public ValidateEmailAttribute()
        {
            LicensingManager.SetLicenseKey("FHbrz2C/8XTEPkmsVzPzPuYvZ2XNoMDfMEdJKJdvGlwPpkAgNwQMVT+Ae1ZSY8QbQpm+7g==");
        }
        public override bool IsValid(object value)
        {
            using (var engine = new VerificationEngine())
            {
                var result = engine.Run(value.ToString(), VerificationLevel.Syntax).Result;
                return result.LastStatus == VerificationStatus.Success;
            }

            // return base.IsValid(value);
        }

       
    }
}