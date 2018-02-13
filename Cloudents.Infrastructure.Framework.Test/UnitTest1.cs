﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;

namespace Cloudents.Infrastructure.Framework.Test
{
    [TestClass]
    public class UnitTest1
    {
        InMemoryDatabaseTest db = new InMemoryDatabaseTest();

        [TestMethod]
        public void test_sqlite_database_using_fluent_nhibernate_directly()
        {
            using (ISession session = db.OpenSession())
            {
                //session.Save(new Teacher() { Name = "Adam" });
                //session.Save(new Teacher() { Name = "Joe" });

                //session.Save(new Class() { Name = "MATH 101", TeacherId = 1 });


                //var query = (from t in session.Query<Teacher>()
                //    join c in session.Query<Class>() on t.Id equals c.TeacherId
                //    select t).ToList();

                //Assert.AreEqual(1, query.Count);
            }
        }

    }
}
