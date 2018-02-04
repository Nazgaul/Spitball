using System;
using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class LibraryNodeApproveAccessCommandHandler : ICommandHandlerAsync<LibraryNodeApproveAccessCommand>
    {
        private readonly IUserLibraryRelRepository m_UserLibraryRepository;
        private readonly ILibraryRepository _libraryRepository;
        private readonly IQueueProvider _queueProvider;

        public LibraryNodeApproveAccessCommandHandler(IUserLibraryRelRepository userLibraryRepository, IQueueProvider queueProvider, ILibraryRepository libraryRepository)
        {
            m_UserLibraryRepository = userLibraryRepository;
            _queueProvider = queueProvider;
            _libraryRepository = libraryRepository;
        }

        public async Task HandleAsync(LibraryNodeApproveAccessCommand message)
        {
            var userAdminToLibraryRel = m_UserLibraryRepository.GetUserLibraryRelationship(message.UserId, message.DepartmentId);
            if (userAdminToLibraryRel == null || userAdminToLibraryRel.UserType != Infrastructure.Enums.UserLibraryRelationType.Owner)
            {
                throw new UnauthorizedAccessException();
            }

            var userToLibraryRel = m_UserLibraryRepository.GetUserLibraryRelationship(message.ApprovedUserId, message.DepartmentId);

            if (userToLibraryRel == null)
            {
                throw new ArgumentException();
            }
            if (userToLibraryRel.UserType != Infrastructure.Enums.UserLibraryRelationType.Pending)
            {
                throw new ArgumentException();
            }

            userToLibraryRel.UserType = Infrastructure.Enums.UserLibraryRelationType.Subscribe;
            m_UserLibraryRepository.Save(userToLibraryRel, true);
            _libraryRepository.UpdateElementToIsDirty(message.DepartmentId);
            var library = _libraryRepository.Load(message.DepartmentId);
            await _queueProvider.InsertMessageToMailNewAsync(new AccessApprovedData(userToLibraryRel.User.Email, userToLibraryRel.User.Culture, library.Name));
        }
    }
}
