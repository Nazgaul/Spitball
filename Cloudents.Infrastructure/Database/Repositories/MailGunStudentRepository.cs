﻿using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using NHibernate;

namespace Cloudents.Infrastructure.Database.Repositories
{
    public class MailGunStudentRepository : NHibernateRepository<MailGunStudent> , IMailGunStudentRepository
    {
        public MailGunStudentRepository(ISession session) : base(session)
        {
        }
    }
}