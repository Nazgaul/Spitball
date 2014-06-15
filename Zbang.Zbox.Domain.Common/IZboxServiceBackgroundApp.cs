using Zbang.Zbox.Domain.Commands;

namespace Zbang.Zbox.Domain.Common
{
    public interface IZboxServiceBackgroundApp
    {
        void CreateUniversity(CreateUniversityCommand command);
    }
}
