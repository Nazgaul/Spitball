using System.ComponentModel.DataAnnotations;
using Cloudents.Command.Courses;
using FluentValidation;

namespace Cloudents.Web.Models
{
    //public class UpdateCourseLandingPage
    //{
    //    public HeroSection Hero { get; set; }

    //}
    

    public class HeroSectionValidator : AbstractValidator<HeroSection> {
        public HeroSectionValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty();
            
            //RuleFor(x => x.Name).Length(0, 10);
            //RuleFor(x => x.Email).EmailAddress();
            //RuleFor(x => x.Age).InclusiveBetween(18, 60);
        }
    }
   
}