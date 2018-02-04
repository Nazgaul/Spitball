using System;
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
        private readonly IUserRepository _userRepository;
        private readonly IRepository<Library> _libraryRepository;
        private readonly IGuidIdGenerator _guidGenerator;
        private readonly IRepository<UserLibraryRel> m_UserLibraryRelRepository;
        private readonly IQueueProvider _queueProvider;
        public RequestAccessLibraryNodeCommandHandler( IUserRepository userRepository, IRepository<Library> libraryRepository, IGuidIdGenerator guidGenerator, IRepository<UserLibraryRel> userLibraryRelRepository, IQueueProvider queueProvider)
        {
            _userRepository = userRepository;
            _libraryRepository = libraryRepository;
            _guidGenerator = guidGenerator;
            m_UserLibraryRelRepository = userLibraryRelRepository;
            _queueProvider = queueProvider;
        }

        public async Task HandleAsync(RequestAccessLibraryNodeCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var userToLibraryRel = _userRepository.GetUserToDepartmentRelationShipType(message.UserId, message.DepartmentId);
            if (userToLibraryRel != Infrastructure.Enums.UserLibraryRelationType.None)
            {
                return;
            }
            var user = _userRepository.Load(message.UserId);
            var library = _libraryRepository.Load(message.DepartmentId);
            if (library.Parent != null)
            {
                throw new ArgumentException();
            }
            var lib = new UserLibraryRel(_guidGenerator.GetId(), user, library,
                Infrastructure.Enums.UserLibraryRelationType.Pending);

            await _queueProvider.InsertMessageToMailNewAsync(new RequestAccessData(library.CreatedUser.Email,library.CreatedUser.Culture,user.Name, user.ImageLarge, library.Name));
            m_UserLibraryRelRepository.Save(lib);
        }
    }
}
