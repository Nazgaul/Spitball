using Zbang.Zbox.Domain.CommandHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Domain.Commands;

namespace TestProject
{
    
    
    /// <summary>
    ///This is a test class for SendMessageToEmailCommandHandlerTest and is intended
    ///to contain all SendMessageToEmailCommandHandlerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SendMessageToEmailCommandHandlerTest
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
        ///A test for SendMessageToEmailCommandHandler Constructor
        ///</summary>
        [TestMethod()]
        public void SendMessageToEmailCommandHandlerConstructorTest()
        {
            IQueueProvider queueProvider = null; // TODO: Initialize to an appropriate value
            IUserRepository userRepository = null; // TODO: Initialize to an appropriate value
            SendMessageToEmailCommandHandler target = new SendMessageToEmailCommandHandler(queueProvider, userRepository);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Execute
        ///</summary>
        [TestMethod()]
        public void ExecuteTest()
        {
            IQueueProvider queueProvider = null; // TODO: Initialize to an appropriate value
            IUserRepository userRepository = null; // TODO: Initialize to an appropriate value
            SendMessageToEmailCommandHandler target = new SendMessageToEmailCommandHandler(queueProvider, userRepository); // TODO: Initialize to an appropriate value
            SendMessageToEmailCommand command = null; // TODO: Initialize to an appropriate value
            SendMessageToEmailCommandResult expected = null; // TODO: Initialize to an appropriate value
            SendMessageToEmailCommandResult actual;
            actual = target.Execute(command);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
