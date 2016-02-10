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

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class RequestAccessLibraryNodeCommandHandler : ICommandHandlerAsync<RequestAccessLibraryNodeCommand>
    {
        private readonly IUserRepository m_UserRepository;
        private readonly IRepository<Library> m_LibraryRepository;
        private readonly IGuidIdGenerator m_GuidGenerator;
        private readonly IRepository<UserLibraryRel> m_UserLibraryRelRepository;
        public RequestAccessLibraryNodeCommandHandler( IUserRepository userRepository, IRepository<Library> libraryRepository, IGuidIdGenerator guidGenerator, IRepository<UserLibraryRel> userLibraryRelRepository)
        {
            m_UserRepository = userRepository;
            m_LibraryRepository = libraryRepository;
            m_GuidGenerator = guidGenerator;
            m_UserLibraryRelRepository = userLibraryRelRepository;
        }

        public Task HandleAsync(RequestAccessLibraryNodeCommand message)
        {
            if (message == null) throw new ArgumentNullException("message");
            var userToLibraryRel = m_UserRepository.GetUserToDepartmentRelationShipType(message.UserId, message.DepartmentId);
            if (userToLibraryRel != Infrastructure.Enums.UserLibraryRelationType.None)
            {
                return Task.FromResult<object>(null);
            }
            var user = m_UserRepository.Load(message.UserId);
            var library = m_LibraryRepository.Load(message.DepartmentId);
            if (library.Parent != null)
            {
                throw new ArgumentException();
            }
            var lib = new UserLibraryRel(m_GuidGenerator.GetId(), user, library,
                Infrastructure.Enums.UserLibraryRelationType.Pending);

            m_UserLibraryRelRepository.Save(lib);
            return Task.FromResult<object>(null);
        }
    }
}
