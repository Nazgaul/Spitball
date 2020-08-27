using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Web.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Cloudents.Web.Models
{
    public class CreateCourseRequest : IValidatableObject
    {
        private IEnumerable<CreateLiveStudyRoomRequest> _studyRooms;
        private IEnumerable<CreateDocumentRequest> _documents;

        [Required]
        public string Name { get; set; }

        [Range(0,double.MaxValue)]
        public double Price { get; set; }

        public double? SubscriptionPrice { get; set; }

        [Required]
        public string Description { get; set; }

        public string? Image { get; set; }

        public IEnumerable<CreateLiveStudyRoomRequest> StudyRooms
        {
            get => _studyRooms;
            set
            {
                if (value == null)
                {
                    _studyRooms = Enumerable.Empty<CreateLiveStudyRoomRequest>();
                }

                _studyRooms = value;
            }
        }

        public IEnumerable<CreateDocumentRequest> Documents
        {
            get => _documents;
            set
            {
                if (value == null)
                {
                    _documents = Enumerable.Empty<CreateDocumentRequest>();
                }

                _documents = value;
            }
        }

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

            if (!StudyRooms.Any() && !Documents.Any())
            {
                yield return new ValidationResult("You must set a live class or upload content to make an active course");

            }

            if (StudyRooms.GroupBy(g => g.Date).Any(w => w.Count() > 1))
            {
                yield return new ValidationResult("Can't have the same date time for study room");
            }
        }
    }
}