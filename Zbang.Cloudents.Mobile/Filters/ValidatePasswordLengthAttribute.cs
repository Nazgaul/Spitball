using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.Security;
using Zbang.Cloudents.Mobile.Models.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Filters
{
    public sealed class ValidatePasswordLengthAttribute : ValidationAttribute, IClientValidatable
    {
        //private static string _defaultErrorMessage = ValidatePasswordResources.MustBeAtLeast;
        private readonly int m_MinCharacters = Membership.Provider.MinRequiredPasswordLength;

        public override string FormatErrorMessage(string name)
        {
            ValidatePasswordResources.Culture = System.Threading.Thread.CurrentThread.CurrentCulture;

            //Resources.ValidatePasswordResources.MustBeAtLeast
            return String.Format(ErrorMessageString,
                 m_MinCharacters);
        }

        public override bool IsValid(object value)
        {
            var valueAsString = value as string;
            if (string.IsNullOrEmpty(valueAsString))
                return true;
            return valueAsString.Length >= m_MinCharacters;

            // return (valueAsString != null && valueAsString.Length >= _minCharacters);
        }



        public System.Collections.Generic.IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
             return new[]{      
                 new ModelClientValidationStringLengthRule(FormatErrorMessage(metadata.GetDisplayName())
                     , m_MinCharacters, int.MaxValue) 
             };// - See more at: http://timjames.me/mvc-3-password-length-dataannotation#sthash.Aebh0qbg.dpuf
        }
    }
}