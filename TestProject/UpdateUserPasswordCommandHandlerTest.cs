using Zbang.Zbox.Domain.CommandHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain;

namespace TestProject
{
    
    
    /// <summary>
    ///This is a test class for UpdateUserPasswordCommandHandlerTest and is intended
    ///to contain all UpdateUserPasswordCommandHandlerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UpdateUserPasswordCommandHandlerTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for UpdateUserPasswordCommandHandler Constructor
        ///</summary>
        [TestMethod()]
        public void UpdateUserPasswordCommandHandlerConstructorTest()
        {
            IUserRepository userRepository = null; // TODO: Initialize to an appropriate value
            IMembershipService membershipService = null; // TODO: Initialize to an appropriate value
            UpdateUserPasswordCommandHandler target = new UpdateUserPasswordCommandHandler(userRepository, membershipService);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for ChangeUserPassword
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Zbang.Zbox.Domain.CommandHandlers.dll")]
        public void ChangeUserPasswordTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            UpdateUserPasswordCommandHandler_Accessor target = new UpdateUserPasswordCommandHandler_Accessor(param0); // TODO: Initialize to an appropriate value
            UpdateUserPasswordCommand command = null; // TODO: Initialize to an appropriate value
            User user = null; // TODO: Initialize to an appropriate value
            target.ChangeUserPassword(command, user);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Execute
        ///</summary>
        [TestMethod()]
        public void ExecuteTest()
        {
            IUserRepository userRepository = null; // TODO: Initialize to an appropriate value
            IMembershipService membershipService = null; // TODO: Initialize to an appropriate value
            UpdateUserPasswordCommandHandler target = new UpdateUserPasswordCommandHandler(userRepository, membershipService); // TODO: Initialize to an appropriate value
            UpdateUserPasswordCommand command = null; // TODO: Initialize to an appropriate value
            UpdateUserCommandResult expected = null; // TODO: Initialize to an appropriate value
            UpdateUserCommandResult actual;
            actual = target.Execute(command);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IsUserRegisteredLocaly
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Zbang.Zbox.Domain.CommandHandlers.dll")]
        public void IsUserRegisteredLocalyTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            UpdateUserPasswordCommandHandler_Accessor target = new UpdateUserPasswordCommandHandler_Accessor(param0); // TODO: Initialize to an appropriate value
            User user = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.IsUserRegisteredLocaly(user);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for RegisterUserLocally
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Zbang.Zbox.Domain.CommandHandlers.dll")]
        public void RegisterUserLocallyTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            UpdateUserPasswordCommandHandler_Accessor target = new UpdateUserPasswordCommandHandler_Accessor(param0); // TODO: Initialize to an appropriate value
            UpdateUserPasswordCommand command = null; // TODO: Initialize to an appropriate value
            User user = null; // TODO: Initialize to an appropriate value
            target.RegisterUserLocally(command, user);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
