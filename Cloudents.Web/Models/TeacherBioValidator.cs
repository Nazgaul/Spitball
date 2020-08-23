using Cloudents.Command.Courses;
using FluentValidation;

namespace Cloudents.Web.Models
{
    public class TeacherBioValidator : AbstractValidator<TeacherBio>
    {
        public TeacherBioValidator()
        {
            //RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            //RuleFor(x => x.Text).NotEmpty();
        }
    }
}