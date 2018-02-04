using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteNodeFromLibraryCommandHandler: ICommandHandler<DeleteNodeFromLibraryCommand>
    {
        private readonly IRepository<Library> _libraryRepository;
        public DeleteNodeFromLibraryCommandHandler(IRepository<Library> libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        public void Handle(DeleteNodeFromLibraryCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var node = _libraryRepository.Get(message.NodeId);
            if (node == null)
            {
                throw new NullReferenceException("node");
            }

            if (node.University.Id != message.UniversityId)
            {
                throw new UnauthorizedAccessException("node is not connected to university");
            }

            if (node.AmountOfNodes > 0)
            {
                throw new UnauthorizedAccessException("node is not empty");
            }

            if (node.CheckIfBoxesExists())
            {
                throw new UnauthorizedAccessException("node is not empty");
            }

            var parent = node.Parent;
            if (parent != null)
            {
                parent.AmountOfNodes--;

                _libraryRepository.Save(parent);
            }

            _libraryRepository.Delete(node);
        }
    }
}
