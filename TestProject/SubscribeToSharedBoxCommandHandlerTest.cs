using Zbang.Zbox.Domain.CommandHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Zbang.Zbox.Domain;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Domain.Commands;

namespace TestProject
{
    
    
    /// <summary>
    ///This is a test class for SubscribeToSharedBoxCommandHandlerTest and is intended
    ///to contain all SubscribeToSharedBoxCommandHandlerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SubscribeToSharedBoxCommandHandlerTest
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
        ///A test for SubscribeToSharedBoxCommandHandler Constructor
        ///</summary>
        [TestMethod()]
        public void SubscribeToSharedBoxCommandHandlerConstructorTest()
        {
            IRepository<Box> boxRepository = null; // TODO: Initialize to an appropriate value
            IUserRepository userRepository = null; // TODO: Initialize to an appropriate value
            IRepository<UserBoxRel> userBoxRelRepository = null; // TODO: Initialize to an appropriate value
            IQueueProvider queueProvider = null; // TODO: Initialize to an appropriate value
            SubscribeToSharedBoxCommandHandler target = new SubscribeToSharedBoxCommandHandler(boxRepository, userRepository, userBoxRelRepository, queueProvider);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Execute
        ///</summary>
        [TestMethod()]
        public void ExecuteTest()
        {
            IRepository<Box> boxRepository = null; // TODO: Initialize to an appropriate value
            IUserRepository userRepository = null; // TODO: Initialize to an appropriate value
            IRepository<UserBoxRel> userBoxRelRepository = null; // TODO: Initialize to an appropriate value
            IQueueProvider queueProvider = null; // TODO: Initialize to an appropriate value
            SubscribeToSharedBoxCommandHandler target = new SubscribeToSharedBoxCommandHandler(boxRepository, userRepository, userBoxRelRepository, queueProvider); // TODO: Initialize to an appropriate value
            SubscribeToSharedBoxCommand command = null; // TODO: Initialize to an appropriate value
            SubscribeToSharedBoxCommandResult expected = null; // TODO: Initialize to an appropriate value
            SubscribeToSharedBoxCommandResult actual;
            actual = target.Execute(command);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SendNotification
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Zbang.Zbox.Domain.CommandHandlers.dll")]
        public void SendNotificationTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            SubscribeToSharedBoxCommandHandler_Accessor target = new SubscribeToSharedBoxCommandHandler_Accessor(param0); // TODO: Initialize to an appropriate value
            User user = null; // TODO: Initialize to an appropriate value
            Box box = null; // TODO: Initialize to an appropriate value
            target.SendNotification(user, box);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
