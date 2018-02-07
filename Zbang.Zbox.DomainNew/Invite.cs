using System;
using Zbang.Zbox.Infrastructure.Repositories;

namespace Zbang.Zbox.Domain
{
    public abstract class Invite : ISoftDelete
    {
        protected Invite()
        {
            IsDeleted = false;
        }

        protected Invite(Guid id, User sender, string image, string userName, string email)
            :this()
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

        //public abstract ReputationAction GiveAction();
        public abstract string UrlToRedirect();


        public bool IsDeleted { get; set; }

        public void DeleteAssociation()
        {
        }
    }
}
