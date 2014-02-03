using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Mail.Parameters;

namespace Zbang.Zbox.Domain.CommandHandlers
{
    public class RequestSubscriptionCommandHandler : ICommandHandler<RequestSubscriptionCommand, RequestSubscriptionCommandResult>
    {
        //Fields                
        private IRepository<Box> m_boxRepository;
        private IUserRepository m_userRepository;
        private IMailManager m_MailManager;

        //Ctors
        public RequestSubscriptionCommandHandler(IMailManager mailManager, IRepository<Box> boxRepository, IUserRepository userRepository
            )
        {
            m_MailManager = mailManager;
            m_userRepository = userRepository;
            m_boxRepository = boxRepository;
        }

        //Methods
        public RequestSubscriptionCommandResult Execute(RequestSubscriptionCommand command)
        {



            User requestingUser = m_userRepository.Get(command.RequestingId);
            if (requestingUser == null)
                throw new ArgumentException("user does not exist");

            Box box = m_boxRepository.Get(command.BoxId);

            if (box == null)
                throw new ArgumentException("this box no longer exists");

            User boxOwner = box.UserBoxRel.Where(w => w.UserType == UserType.owner).SingleOrDefault().User;



            //Guid ownerId = box.Storage.UserId;
            //User boxOwner = m_userRepository.Get(ownerId);
            if (boxOwner == null)
                throw new InvalidOperationException("Couldn't find box owner");

            //Friend friend = m_friendRepository.GetFriendByEmail(box.Storage.UserId, requestingUser.Email);
            //if (friend != null)
            //{
            //    IEnumerable<Invitation> invitations = m_InvitatationRepository.GetBoxInvitations(box, friend);

            //    if (invitations.Count() > 0)
            //        throw new ArgumentException("Requesting user is already a friend and has a pending invitation to subscribe to this box - request to subscribe is not needed");
            //}

            string approvalUri = command.ApprovalUri;
            approvalUri = approvalUri.Replace("<BoxOwnerId>", boxOwner.Id.ToString());
            //CreateMailParams parameters = new CreateMailParams
            //{
            //BoxOwner = boxOwner.Email,
            //UserName = requestingUser.Email,
            //BoxName = box.BoxName,
            //VerificationUri = approvalUri

            //};
            //bool success = m_MailManager.SendEmail(parameters, CreateMailBase.SubscribtionRequest, boxOwner.Email);

            //if (!success)
            throw new InvalidOperationException("Failed to send message to box owner");

            //RequestSubscriptionCommandResult result = new RequestSubscriptionCommandResult();

            //return result;
        }
    }
}
