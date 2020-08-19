using Cloudents.Command.Courses;
using FluentValidation;

namespace Cloudents.Web.Models
{
    //public class UpdateCourseLandingPage
    //{
    //    public HeroSection Hero { get; set; }

    //}


    public class UpdateCourseLandingCommandValidator : AbstractValidator<UpdateCourseLandingCommand>
    {
        public UpdateCourseLandingCommandValidator()
        {
            RuleFor(x => x.HeroSection)
                .SetValidator(new HeroSectionValidator()).When(w => w.HeroSection != null);

            RuleForEach(x => x.LiveClassSection)
                .SetValidator(new LiveClassSectionValidator()).When(w => w.LiveClassSection != null);


            RuleFor(x => x.ClassContent)
                .SetValidator(new ClassContentValidator()).When(w => w.ClassContent != null);

            RuleFor(x => x.TeacherBio)
                .SetValidator(new TeacherBioValidator()).When(w => w.TeacherBio != null);

        }
    }
}