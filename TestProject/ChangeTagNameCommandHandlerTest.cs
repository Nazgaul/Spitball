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
    ///This is a test class for ChangeTagNameCommandHandlerTest and is intended
    ///to contain all ChangeTagNameCommandHandlerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ChangeTagNameCommandHandlerTest
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
        ///A test for ChangeTagNameCommandHandler Constructor
        ///</summary>
        [TestMethod()]
        public void ChangeTagNameCommandHandlerConstructorTest()
        {
            IRepository<Tag> tagRepository = null; // TODO: Initialize to an appropriate value
            IUserRepository userRepository = null; // TODO: Initialize to an appropriate value
            ChangeTagNameCommandHandler target = new ChangeTagNameCommandHandler(tagRepository, userRepository);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Execute
        ///</summary>
        [TestMethod()]
        public void ExecuteTest()
        {
            IRepository<Tag> tagRepository = null; // TODO: Initialize to an appropriate value
            IUserRepository userRepository = null; // TODO: Initialize to an appropriate value
            ChangeTagNameCommandHandler target = new ChangeTagNameCommandHandler(tagRepository, userRepository); // TODO: Initialize to an appropriate value
            ChangeTagNameCommand command = null; // TODO: Initialize to an appropriate value
            ChangeTagNameCommandResult expected = null; // TODO: Initialize to an appropriate value
            ChangeTagNameCommandResult actual;
            actual = target.Execute(command);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
