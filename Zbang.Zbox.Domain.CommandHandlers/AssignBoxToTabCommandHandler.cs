using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Exceptions;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class AssignBoxToTabCommandHandler : ICommandHandler<AssignBoxToTabCommand>
    {
        private readonly IUserBoxRelRepository m_UserBoxRelRepository;
        private readonly IBoxTabRepository m_BoxTagRepository;

        public AssignBoxToTabCommandHandler(IUserBoxRelRepository userBoxRelRepository, IBoxTabRepository boxTagRepository
            )
        {
            m_UserBoxRelRepository = userBoxRelRepository;
            m_BoxTagRepository = boxTagRepository;
           
        }
        public void Handle(AssignBoxToTabCommand message)
        {
            Throw.OnNull(message, "message");


            var userBoxRel = m_UserBoxRelRepository.GetUserBoxRelationship(message.UserId, message.BoxId);
            Throw.OnNull(userBoxRel, "userBoxRel");

            if (userBoxRel.UserRelationshipType == Infrastructure.Enums.UserRelationshipType.None ||
                userBoxRel.UserRelationshipType == Infrastructure.Enums.UserRelationshipType.Invite)
            {
                throw new UnauthorizedAccessException("You need to follow the box");
            }
            

            var boxTab = m_BoxTagRepository.Get(message.TabId);
            Throw.OnNull(boxTab, "boxTab");

            boxTab.AddBoxToTag(userBoxRel);

            m_BoxTagRepository.Save(boxTab);


        }
    }
}
