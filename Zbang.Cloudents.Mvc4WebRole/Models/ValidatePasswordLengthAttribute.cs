using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.Security;
using Zbang.Cloudents.Mvc4WebRole.Models.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public sealed class ValidatePasswordLengthAttribute : ValidationAttribute, IClientValidatable
    {
        //private static string _defaultErrorMessage = ValidatePasswordResources.MustBeAtLeast;
        private readonly int _minCharacters = Membership.Provider.MinRequiredPasswordLength;

        public override string FormatErrorMessage(string name)
        {
            ValidatePasswordResources.Culture = System.Threading.Thread.CurrentThread.CurrentCulture;

            //Resources.ValidatePasswordResources.MustBeAtLeast
            return String.Format(ErrorMessageString,
                 _minCharacters);
        }

        public override bool IsValid(object value)
        {
            var valueAsString = value as string;
            if (string.IsNullOrEmpty(valueAsString))
                return true;
            return valueAsString.Length >= _minCharacters;

            // return (valueAsString != null && valueAsString.Length >= _minCharacters);
        }



        public System.Collections.Generic.IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
             return new[]{      
                 new ModelClientValidationStringLengthRule(FormatErrorMessage(metadata.GetDisplayName())
                     , _minCharacters, int.MaxValue) 
             };// - See more at: http://timjames.me/mvc-3-password-length-dataannotation#sthash.Aebh0qbg.dpuf
        }
    }
}