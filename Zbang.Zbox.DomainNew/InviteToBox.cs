using System;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.Domain
{
    public class InviteToBox : Invite
    {
        protected InviteToBox()
        {

        }

        public InviteToBox(Guid id, User sender, Box box, string userName, string email)
            : base(id, sender, null, userName, email)
        {
            Box = box;
            New = true;
            Read = false;
        }

        public virtual Box Box { get; protected set; }

       

        public virtual bool Read { get; protected set; }
        public virtual bool New { get; protected set; }

        //public override ReputationAction GiveAction()
        //{
        //    return ReputationAction.InviteToBox;
        //}

        public override string UrlToRedirect()
        {
            return Box.Url;
        }

        public virtual void UpdateMessageAsRead()
        {
            Read = true;
        }

        public void UpdateMessageAsOld()
        {
            New = false;
        }
    }
}