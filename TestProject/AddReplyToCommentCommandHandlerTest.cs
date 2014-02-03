using Zbang.Zbox.Domain.CommandHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Domain;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Domain.Commands;

namespace TestProject
{
    
    
    /// <summary>
    ///This is a test class for AddReplyToCommentCommandHandlerTest and is intended
    ///to contain all AddReplyToCommentCommandHandlerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AddReplyToCommentCommandHandlerTest
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
        ///A test for AddReplyToCommentCommandHandler Constructor
        ///</summary>
        [TestMethod()]
        public void AddReplyToCommentCommandHandlerConstructorTest()
        {
            IUserRepository userRepository = null; // TODO: Initialize to an appropriate value
            IRepository<Comment> targetRepository = null; // TODO: Initialize to an appropriate value
            AddReplyToCommentCommandHandler target = new AddReplyToCommentCommandHandler(userRepository, targetRepository);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Execute
        ///</summary>
        [TestMethod()]
        public void ExecuteTest()
        {
            IUserRepository userRepository = null; // TODO: Initialize to an appropriate value
            IRepository<Comment> targetRepository = null; // TODO: Initialize to an appropriate value
            AddReplyToCommentCommandHandler target = new AddReplyToCommentCommandHandler(userRepository, targetRepository); // TODO: Initialize to an appropriate value
            AddReplyToCommentCommand command = null; // TODO: Initialize to an appropriate value
            AddReplyToCommentCommandResult expected = null; // TODO: Initialize to an appropriate value
            AddReplyToCommentCommandResult actual;
            actual = target.Execute(command);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
