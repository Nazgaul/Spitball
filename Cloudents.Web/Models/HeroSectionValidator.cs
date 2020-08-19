using Cloudents.Command.Courses;
using FluentValidation;

namespace Cloudents.Web.Models
{
    public class HeroSectionValidator : AbstractValidator<HeroSection>
    {
        public HeroSectionValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}