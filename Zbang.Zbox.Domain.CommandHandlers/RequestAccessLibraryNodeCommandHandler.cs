using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.IdGenerator;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class RequestAccessLibraryNodeCommandHandler : ICommandHandlerAsync<RequestAccessLibraryNodeCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<Library> m_LibraryRepository;
        private readonly IGuidIdGenerator m_GuidGenerator;
        private readonly IRepository<UserLibraryRel> m_UserLibraryRelRepository;
        private readonly IQueueProvider m_QueueProvider;
        public RequestAccessLibraryNodeCommandHandler( IUserRepository userRepository, IRepository<Library> libraryRepository, IGuidIdGenerator guidGenerator, IRepository<UserLibraryRel> userLibraryRelRepository, IQueueProvider queueProvider)
        {
            m_UserRepository = userRepository;
            m_LibraryRepository = libraryRepository;
            m_GuidGenerator = guidGenerator;
            m_UserLibraryRelRepository = userLibraryRelRepository;
            m_QueueProvider = queueProvider;
        }

        public async Task HandleAsync(RequestAccessLibraryNodeCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");
            var userToLibraryRel = m_UserRepository.GetUserToDepartmentRelationShipType(message.UserId, message.DepartmentId);
            if (userToLibraryRel != Infrastructure.Enums.UserLibraryRelationType.None)
            {
                return;
            }
            var user = m_UserRepository.Load(message.UserId);
            var library = m_LibraryRepository.Load(message.DepartmentId);
            if (library.Parent != null)
            {
                throw new ArgumentException();
            }
            var lib = new UserLibraryRel(m_GuidGenerator.GetId(), user, library,
                Infrastructure.Enums.UserLibraryRelationType.Pending);

            await m_QueueProvider.InsertMessageToMailNewAsync(new RequestAccessData(library.CreatedUser.Email,library.CreatedUser.Culture));
            m_UserLibraryRelRepository.Save(lib);
        }
    }
}
