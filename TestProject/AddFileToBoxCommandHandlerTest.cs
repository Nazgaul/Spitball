using Zbang.Zbox.Domain.CommandHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Domain;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Domain.Commands;

namespace TestProject
{
    
    
    /// <summary>
    ///This is a test class for AddFileToBoxCommandHandlerTest and is intended
    ///to contain all AddFileToBoxCommandHandlerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AddFileToBoxCommandHandlerTest
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
        ///A test for AddFileToBoxCommandHandler Constructor
        ///</summary>
        [TestMethod()]
        public void AddFileToBoxCommandHandlerConstructorTest()
        {
            IQueueProvider queueProvider = null; // TODO: Initialize to an appropriate value
            IRepository<Box> boxRepository = null; // TODO: Initialize to an appropriate value
            IUserRepository userRepository = null; // TODO: Initialize to an appropriate value
            IActionRepository actionRepository = null; // TODO: Initialize to an appropriate value
            AddFileToBoxCommandHandler target = new AddFileToBoxCommandHandler(queueProvider, boxRepository, userRepository, actionRepository);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for CheckPermission
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Zbang.Zbox.Domain.CommandHandlers.dll")]
        public void CheckPermissionTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            AddFileToBoxCommandHandler_Accessor target = new AddFileToBoxCommandHandler_Accessor(param0); // TODO: Initialize to an appropriate value
            UserType userType = new UserType(); // TODO: Initialize to an appropriate value
            target.CheckPermission(userType);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Execute
        ///</summary>
        [TestMethod()]
        public void ExecuteTest()
        {
            IQueueProvider queueProvider = null; // TODO: Initialize to an appropriate value
            IRepository<Box> boxRepository = null; // TODO: Initialize to an appropriate value
            IUserRepository userRepository = null; // TODO: Initialize to an appropriate value
            IActionRepository actionRepository = null; // TODO: Initialize to an appropriate value
            AddFileToBoxCommandHandler target = new AddFileToBoxCommandHandler(queueProvider, boxRepository, userRepository, actionRepository); // TODO: Initialize to an appropriate value
            AddFileToBoxCommand command = null; // TODO: Initialize to an appropriate value
            AddFileToBoxCommandResult expected = null; // TODO: Initialize to an appropriate value
            AddFileToBoxCommandResult actual;
            actual = target.Execute(command);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for TriggerCacheDocument
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Zbang.Zbox.Domain.CommandHandlers.dll")]
        public void TriggerCacheDocumentTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            AddFileToBoxCommandHandler_Accessor target = new AddFileToBoxCommandHandler_Accessor(param0); // TODO: Initialize to an appropriate value
            AddFileToBoxCommand command = null; // TODO: Initialize to an appropriate value
            target.TriggerCacheDocument(command);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for TriggerGenerateThumbnail
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Zbang.Zbox.Domain.CommandHandlers.dll")]
        public void TriggerGenerateThumbnailTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            AddFileToBoxCommandHandler_Accessor target = new AddFileToBoxCommandHandler_Accessor(param0); // TODO: Initialize to an appropriate value
            string blobName = string.Empty; // TODO: Initialize to an appropriate value
            long fileId = 0; // TODO: Initialize to an appropriate value
            target.TriggerGenerateThumbnail(blobName, fileId);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for TriggerNotification
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Zbang.Zbox.Domain.CommandHandlers.dll")]
        public void TriggerNotificationTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            AddFileToBoxCommandHandler_Accessor target = new AddFileToBoxCommandHandler_Accessor(param0); // TODO: Initialize to an appropriate value
            Box box = null; // TODO: Initialize to an appropriate value
            User user = null; // TODO: Initialize to an appropriate value
            Item item = null; // TODO: Initialize to an appropriate value
            target.TriggerNotification(box, user, item);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
