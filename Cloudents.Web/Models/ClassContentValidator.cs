using Cloudents.Command.Courses;
using FluentValidation;

namespace Cloudents.Web.Models
{
    public class ClassContentValidator : AbstractValidator<ClassContent>
    {
        public ClassContentValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Text).NotEmpty();
        }
    }
}