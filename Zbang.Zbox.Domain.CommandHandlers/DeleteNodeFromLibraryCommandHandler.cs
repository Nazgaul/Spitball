using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Exceptions;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteNodeFromLibraryCommandHandler: ICommandHandler<DeleteNodeFromLibraryCommand>
    {
        private readonly IRepository<Library> m_LibraryRepository;
        public DeleteNodeFromLibraryCommandHandler(IRepository<Library> libraryRepository)
        {
            m_LibraryRepository = libraryRepository;
        }
        public void Handle(DeleteNodeFromLibraryCommand message)
        {
            Throw.OnNull(message,"message");
            var node = m_LibraryRepository.Get(message.NodeId);
            Throw.OnNull(node, "node");

            if (node.University.Id != message.UniversityId)
            {
                throw new UnauthorizedAccessException("node is not connected to university");
            }
            


            if (node.AmountOfNodes > 0)
            {
                throw new UnauthorizedAccessException("node is not empty");
            }
            if (node.Boxes.Count > 0)
            {
                throw new UnauthorizedAccessException("node is not empty");
            }
            
            var parent = node.Parent;
            if (parent != null)
            {
                parent.AmountOfNodes--;

                m_LibraryRepository.Save(parent);
            }

            m_LibraryRepository.Delete(node);
        }
    }
}
