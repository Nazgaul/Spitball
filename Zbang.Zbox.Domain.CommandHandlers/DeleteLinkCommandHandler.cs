using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Blob;
using Microsoft.WindowsAzure.StorageClient;
using Zbang.Zbox.Infrastructure.EventHandlers;
using Zbang.Zbox.Domain.Events;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class DeleteLinkCommandHandler : ICommandHandler<DeleteLinkCommand, DeleteLinkCommandResult>
    {
        //Fields        
        private IRepository<Box> m_BoxRepository;
        private IRepository<Link> m_LinkRepository;
        private IEventBus m_EventBus;

        //Ctors
        public DeleteLinkCommandHandler(EventBus eventBus, IRepository<Box> boxRepository, IRepository<Link> linkRepository)
        {
            m_BoxRepository = boxRepository;
            m_LinkRepository = linkRepository;
            m_EventBus = eventBus;
        }

        public DeleteLinkCommandResult Execute(DeleteLinkCommand command)
        {
            Link link = m_LinkRepository.Get(command.LinkId);

            Box box = link.Box;
            UserPermissionSettings permission = box.GetUserPermission(command.UserId);
            if (permission.HasFlag(UserPermissionSettings.Read))
            {
                throw new UnauthorizedAccessException("User is unauthorized to delete link");
            }
            if (!box.DeleteLink(link))
                throw new ArgumentException("Link doesn't exist in box");


            m_BoxRepository.Save(box);
            m_EventBus.Raise<BoxItemDeletedEvent>(new BoxItemDeletedEvent(box.Storage.UserId, box.Id, link.GetType(), link.Url));
            DeleteLinkCommandResult result = new DeleteLinkCommandResult();

            return result;
        }
    }
}
