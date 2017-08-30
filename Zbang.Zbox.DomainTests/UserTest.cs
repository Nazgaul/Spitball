using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zbang.Zbox.Domain;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.DomainTests
{
    [TestClass]
    public class UserTest
    {
        private User m_SomeUser;

        [TestInitialize]
        public void Setup()
        {
            m_SomeUser = new User("some email", "some largeImage", "some first name", "some last name", "en-US", Sex.NotApplicable);
        }

        //[TestMethod]
        //public void UpdateUserUniversity_NullUniversity_ThrowException()
        //{
        //    m_SomeUser.UpdateUserUniversity(null, string.Empty, null, null, null);
        //    Assert.AreEqual(m_SomeUser.University, null);
        //    // Assert.AreEqual(m_SomeUser.UniversityAlias, null);
        //}


        [TestMethod]
        public void ChangeUserRelationShipToBoxType_UserChangeRelationShipToBox_UpdateUserTimeUpdated()
        {
            long someBoxId = 1;
            var someOtherUser = new User("some email2", "some largeImage2", "some first name2", "some last nam2e", "en-US", Sex.NotApplicable);
            var someBox = new PrivateBox("some box", someOtherUser, Infrastructure.Enums.BoxPrivacySetting.MembersOnly, Guid.NewGuid());
            someBox.GetType().GetProperty("Id").SetValue(someBox, someBoxId);

            var someUserBoxRel = new UserBoxRel(m_SomeUser, someBox, Infrastructure.Enums.UserRelationshipType.Invite);
            var updateDateTime = someUserBoxRel.UserTime.UpdateTime;

            m_SomeUser.UserBoxRel.Add(someUserBoxRel);
            m_SomeUser.ChangeUserRelationShipToBoxType(someBox, Infrastructure.Enums.UserRelationshipType.Subscribe);

            Assert.AreNotEqual<DateTime>(updateDateTime, someUserBoxRel.UserTime.UpdateTime);

        }
    }
}
