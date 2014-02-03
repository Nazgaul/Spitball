using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zbang.Zbox.Domain;

namespace Zbang.Zbox.DomainTests
{
    [TestClass]
    public class FriendTest
    {
        private Friend m_Friend1;

        [Microsoft.VisualStudio.TestTools.UnitTesting.TestInitialize]
        public void Setup()
        {
            var owner = CreateSomeUser();
            User friendUser = CreateSomeUser();

            m_Friend1 = new Friend(owner, friendUser);
        }

        [TestMethod]
        public void Equals_ToNull_ReturnsFalse()
        {
            //arrange
            Friend friend2 = null;

            //act
            bool equal = m_Friend1.Equals(friend2);
            
            //assert
            Assert.IsFalse(equal, "shouldn't be equal to null");
        }

        [TestMethod]
        public void Equals_ToSelf_ReturnsTrue()
        {
            //arrange
            Friend friend2 = m_Friend1;
            //act
            bool equal = m_Friend1.Equals(friend2);
            //assert
            Assert.IsTrue(equal, "same instance should be equal to itself");
        }

        [TestMethod]
        public void Equals_ToSameOwner_ReturnsTrue()
        {
            var owner = CreateSomeUser();
            User friendUser = CreateSomeUser();
            
            //arrange
            Friend friend2 = new Friend(owner,friendUser);
            //act
            bool equal = m_Friend1.Equals(friend2);
            //assert
            Assert.IsTrue(equal, "same instance should be equal to itself");
        }

        private User CreateSomeUser()
        {
           return new User("some email", "some user", "some image", "some large image");
        }

        
    }
}
