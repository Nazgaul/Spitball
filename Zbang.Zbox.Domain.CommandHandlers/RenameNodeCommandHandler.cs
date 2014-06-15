using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class RenameNodeCommandHandler : ICommandHandler<RenameNodeCommand>
    {
        private readonly IRepository<Library> m_LibraryRepository;
        public RenameNodeCommandHandler(IRepository<Library> libraryRepository)
        {
            m_LibraryRepository = libraryRepository;
        }
        public void Handle(RenameNodeCommand message)
        {
            Throw.OnNull(message, "message");

            if (string.IsNullOrWhiteSpace(message.NewName))
            {
                throw new NullReferenceException("new name cannot be empty");
            }

            var node = m_LibraryRepository.Get(message.NodeId);
            Throw.OnNull(node, "node");

            if (node.University.Id != message.UniversityId)
            {
                throw new UnauthorizedAccessException("node not connected to university");
            }
            
            node.ChangeName(message.NewName);

            m_LibraryRepository.Save(node);
        }
    }
}
