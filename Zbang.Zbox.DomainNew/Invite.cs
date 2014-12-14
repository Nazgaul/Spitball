using System;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain
{
    public abstract class Invite
    {
        protected Invite()
        {

        }
        protected Invite(Guid id, User sender, string image, string userName, string email)
        {
            Id = id;
            Sender = sender;
            CreationTime = DateTime.UtcNow;
            IsUsed = false;
            Image = image;
            UserName = userName;
            Email = email;
        }

        public virtual Guid Id { get; protected set; }
        public virtual User Sender { get; protected set; }

        public DateTime CreationTime { get; protected set; }

        public virtual bool IsUsed { get; protected set; }

        public string UserName { get; protected set; }

        public string Image { get; protected set; }

        public string Email { get; set; }

        public virtual void UsedInvite()
        {
            IsUsed = true;
        }

        public abstract ReputationAction GiveAction();
        public abstract string UrlToRedirect();

    }


    public class InviteToBox : Invite
    {
        protected InviteToBox()
        {

        }

        public InviteToBox(Guid id, User sender, Box box, UserBoxRel userBoxRel, string image, string userName, string email)
            : base(id, sender, image, userName, email)
        {
            Box = box;
            UserBoxRel = userBoxRel;
            New = true;
            Read = false;
        }
        public virtual Box Box { get; protected set; }
        public virtual UserBoxRel UserBoxRel { get; protected set; }

        public virtual bool Read { get; protected set; }
        public virtual bool New { get; protected set; }

        public override ReputationAction GiveAction()
        {
            return ReputationAction.InviteToBox;
        }

        public override string UrlToRedirect()
        {
            return Box.Url;
        }

        public void UpdateMessageAsRead()
        {
            Read = true;
        }
        public void UpdateMessageAsOld()
        {
            New = false;
        }
    }

    public class InviteToSystem : Invite
    {
        protected InviteToSystem()
        {

        }
        public InviteToSystem(Guid id, User sender, string image, string userName, string email)
            : base(id, sender, image, userName, email)
        {

        }


        public override ReputationAction GiveAction()
        {
            return ReputationAction.Invite;
        }

        public override string UrlToRedirect()
        {
            return UrlConsts.CloudentsUrl;
        }
    }
}
