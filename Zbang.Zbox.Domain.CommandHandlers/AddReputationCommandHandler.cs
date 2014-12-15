using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AddReputationCommandHandler : ICommandHandlerAsync<AddReputationCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<Reputation> m_ReputationRepository;
        private readonly IQueueProvider m_QueueProvider;

        public AddReputationCommandHandler(IUserRepository userRepository,
            IRepository<Reputation> reputationRepository, IQueueProvider queueProvider)
        {
            m_UserRepository = userRepository;
            m_ReputationRepository = reputationRepository;
            m_QueueProvider = queueProvider;
        }
        public Task HandleAsync(AddReputationCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");
            var user = m_UserRepository.Load(message.UserId);
            var reputation = user.AddReputation(Infrastructure.Enums.ReputationAction.ShareFacebook);

            m_ReputationRepository.Save(reputation);
            m_UserRepository.Save(user);
            return m_QueueProvider.InsertMessageToTranactionAsync(new ReputationData(user.Id));
        }
    }
}
