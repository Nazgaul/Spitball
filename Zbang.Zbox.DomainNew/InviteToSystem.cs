﻿using System;
using Zbang.Zbox.Infrastructure.Consts;

namespace Zbang.Zbox.Domain
{
    public class InviteToSystem : Invite
    {
        // ReSharper disable once UnusedMember.Global nhibernate use
        protected InviteToSystem()
        {

        }

        public InviteToSystem(Guid id, User sender, string image, string userName, string email)
            : base(id, sender, image, userName, email)
        {

        }


        //public override ReputationAction GiveAction()
        //{
        //    return ReputationAction.Invite;
        //}

        public override string UrlToRedirect()
        {
            return UrlConst.SystemUrl;
        }
    }
}