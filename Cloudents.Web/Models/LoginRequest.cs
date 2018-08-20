using System.ComponentModel.DataAnnotations;
using System.Web.ModelBinding;
using Cloudents.Web.Binders;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Models
{
    //public class LoginRequest
    //{
    //    [EmailAddress,Required]
    //    public string Email { get; set; }
    //}

    public class SignUserRequest
    {
        public SignUserRequest(string email, bool emailConfirmed, string name)
        {
            Email = email;
            EmailConfirmed = emailConfirmed;
            Name = name;
        }

        public SignUserRequest()
        {
        }

        [EmailAddress, Required]
        public string Email { get; set; }

        //[ModelBinder(typeof(ReturnUrlEntityBinder))]
        //[FromHeader]
        //public string ReturnUrl { get; set; }

        [BindNever]
        public bool EmailConfirmed { get; private set; }

        [BindNever]
        public string Name { get; private set; }
    }
}
