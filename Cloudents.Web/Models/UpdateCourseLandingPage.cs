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

        }
    }

    public class HeroSectionValidator : AbstractValidator<HeroSection>
    {
        public HeroSectionValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
        }
    }

    public class LiveClassSectionValidator : AbstractValidator<LiveClassSection>
    {
        public LiveClassSectionValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public class ClassContentValidator : AbstractValidator<ClassContent>
    {
        public ClassContentValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Text).NotEmpty();
        }
    }
}