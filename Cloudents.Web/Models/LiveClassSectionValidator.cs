using Cloudents.Command.Courses;
using FluentValidation;

namespace Cloudents.Web.Models
{
    public class LiveClassSectionValidator : AbstractValidator<LiveClassSection>
    {
        public LiveClassSectionValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}