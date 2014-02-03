using Zbang.Zbox.Domain.CommandHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Zbang.Zbox.Domain;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Domain.Commands;

namespace TestProject
{
    
    
    /// <summary>
    ///This is a test class for AddTagCommandHandlerTest and is intended
    ///to contain all AddTagCommandHandlerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AddTagCommandHandlerTest
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
        ///A test for AddTagCommandHandler Constructor
        ///</summary>
        [TestMethod()]
        public void AddTagCommandHandlerConstructorTest()
        {
            IRepository<Tag> tagRepository = null; // TODO: Initialize to an appropriate value
            IUserRepository userRepository = null; // TODO: Initialize to an appropriate value
            AddTagCommandHandler target = new AddTagCommandHandler(tagRepository, userRepository);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for AddSubTag
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Zbang.Zbox.Domain.CommandHandlers.dll")]
        public void AddSubTagTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            AddTagCommandHandler_Accessor target = new AddTagCommandHandler_Accessor(param0); // TODO: Initialize to an appropriate value
            long parentId = 0; // TODO: Initialize to an appropriate value
            User user = null; // TODO: Initialize to an appropriate value
            string tagName = string.Empty; // TODO: Initialize to an appropriate value
            Tag parentTag = null; // TODO: Initialize to an appropriate value
            Tag parentTagExpected = null; // TODO: Initialize to an appropriate value
            Tag expected = null; // TODO: Initialize to an appropriate value
            Tag actual;
            actual = target.AddSubTag(parentId, user, tagName, out parentTag);
            Assert.AreEqual(parentTagExpected, parentTag);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for AddTag
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Zbang.Zbox.Domain.CommandHandlers.dll")]
        public void AddTagTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            AddTagCommandHandler_Accessor target = new AddTagCommandHandler_Accessor(param0); // TODO: Initialize to an appropriate value
            User user = null; // TODO: Initialize to an appropriate value
            string tagName = string.Empty; // TODO: Initialize to an appropriate value
            Tag newTag = null; // TODO: Initialize to an appropriate value
            Tag newTagExpected = null; // TODO: Initialize to an appropriate value
            Tag expected = null; // TODO: Initialize to an appropriate value
            Tag actual;
            actual = target.AddTag(user, tagName, out newTag);
            Assert.AreEqual(newTagExpected, newTag);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Execute
        ///</summary>
        [TestMethod()]
        public void ExecuteTest()
        {
            IRepository<Tag> tagRepository = null; // TODO: Initialize to an appropriate value
            IUserRepository userRepository = null; // TODO: Initialize to an appropriate value
            AddTagCommandHandler target = new AddTagCommandHandler(tagRepository, userRepository); // TODO: Initialize to an appropriate value
            AddTagCommand command = null; // TODO: Initialize to an appropriate value
            AddTagCommandResult expected = null; // TODO: Initialize to an appropriate value
            AddTagCommandResult actual;
            actual = target.Execute(command);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
