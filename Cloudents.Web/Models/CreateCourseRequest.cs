using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Web.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Cloudents.Web.Models
{
    public class CreateCourseRequest : IValidatableObject
    {
        [Required]
        public string Name { get; set; }

        [Range(0,int.MaxValue)]
        public int Price { get; set; }

        public int? SubscriptionPrice { get; set; }

        [Required]
        public string Description { get; set; }

        public string? Image { get; set; }

        public IEnumerable<CreateLiveStudyRoomRequest> StudyRooms { get; set; }
        public IEnumerable<CreateDocumentRequest> Documents { get; set; }
        public bool IsPublish { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var httpContext = validationContext.GetService<IHttpContextAccessor>();
            var country = Enumeration.FromDisplayName<Country>(httpContext.HttpContext.User
                .FindFirst(AppClaimsPrincipalFactory.SbCountry).Value);
            if (country == Country.Israel && Price > 0 && Price < 5)
            {
                yield return new ValidationResult("Price should be greater then 5");

            }
        }
    }
}