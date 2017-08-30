using System;
using System.Linq;
using System.Collections.Generic;
using NHibernate.Criterion;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;
using Zbang.Zbox.Infrastructure.Data.Repositories;

namespace Zbang.Zbox.Domain.DataAccess
{
    public class InviteRepository : NHibernateRepository<Invite>, IInviteRepository
    {
        public IEnumerable<Invite> GetUserInvitesToBox(string email,long boxId)
        {
            return UnitOfWork.CurrentSession.QueryOver<InviteToBox>()
                .Where(w => w.Email == email && !w.IsUsed && w.Box.Id == boxId).List();
        }

        public IEnumerable<Invite> GetUserInvites(string email, Guid id)
        {
           return UnitOfWork.CurrentSession.QueryOver<Invite>()
                .Where(w => (w.Email == email || w.Id == id) && !w.IsUsed).List();

        }
        //public InviteToBox GetUserInvite(long userBoxRelId)
        //{
        //    return UnitOfWork.CurrentSession.QueryOver<InviteToBox>()
        //        .Where(w => w.UserBoxRel.Id == userBoxRelId && !w.IsUsed)
        //        .SingleOrDefault();
        //}

        //public IEnumerable<InviteToBox> GetUserInvites(long recipientId)
        //{
        //    var userBoxRels =  UnitOfWork.CurrentSession.QueryOver<UserBoxRel>()
        //        .Where(w => w.User.Id == recipientId).List().ToArray();
        //    return UnitOfWork.CurrentSession.QueryOver<InviteToBox>().Where(Restrictions.On<InviteToBox>(s=>s.UserBoxRel).IsIn(userBoxRels)).List();
        //}
    }
}
