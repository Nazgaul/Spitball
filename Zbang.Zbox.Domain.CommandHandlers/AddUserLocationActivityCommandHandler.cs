using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddUserLocationActivityCommandHandler : ICommandHandlerAsync<AddUserLocationActivityCommand>
    {
        private readonly ILocationProvider m_LocationProvider;
        private readonly IRepository<UserLocationActivity> m_UserLocationActivityRepository;
        private readonly IRepository<User> m_UserRepository;
        private readonly IGuidIdGenerator m_GuidGeneratory;

        public AddUserLocationActivityCommandHandler(ILocationProvider locationProvider, 
            IRepository<UserLocationActivity> userLocationActivityRepository,
            IRepository<User> userRepository, IGuidIdGenerator guidGeneratory)
        {
            m_LocationProvider = locationProvider;
            m_UserLocationActivityRepository = userLocationActivityRepository;
            m_UserRepository = userRepository;
            m_GuidGeneratory = guidGeneratory;
        }

        public async Task HandleAsync(AddUserLocationActivityCommand message)
        {
            var result = await m_LocationProvider.GetLocationDataAsync(message.IpAddress);
            if (result == null)
            {
                return;
            }
            var user = m_UserRepository.Load(message.UserId);
            var locationActivity = new UserLocationActivity(
                m_GuidGeneratory.GetId(),
                user,
                result.Domain,
                result.Latitude,
                result.Longitude,
                result.ZipCode,
                result.Region,
                result.ISP,
                result.City,
                result.Country,
                result.CountryAbbreviation, message.UserAgent);
            m_UserLocationActivityRepository.Save(locationActivity);

            message.Country = result.CountryAbbreviation;

        }
    }
}
