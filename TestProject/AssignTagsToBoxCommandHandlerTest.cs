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
    ///This is a test class for AssignTagsToBoxCommandHandlerTest and is intended
    ///to contain all AssignTagsToBoxCommandHandlerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AssignTagsToBoxCommandHandlerTest
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
        ///A test for AssignTagsToBoxCommandHandler Constructor
        ///</summary>
        [TestMethod()]
        public void AssignTagsToBoxCommandHandlerConstructorTest()
        {
            ITagRepository tagRepository = null; // TODO: Initialize to an appropriate value
            IRepository<Box> boxRepository = null; // TODO: Initialize to an appropriate value
            AssignTagsToBoxCommandHandler target = new AssignTagsToBoxCommandHandler(tagRepository, boxRepository);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Execute
        ///</summary>
        [TestMethod()]
        public void ExecuteTest()
        {
            ITagRepository tagRepository = null; // TODO: Initialize to an appropriate value
            IRepository<Box> boxRepository = null; // TODO: Initialize to an appropriate value
            AssignTagsToBoxCommandHandler target = new AssignTagsToBoxCommandHandler(tagRepository, boxRepository); // TODO: Initialize to an appropriate value
            AssignTagsToBoxCommand command = null; // TODO: Initialize to an appropriate value
            AssignTagsToBoxCommandResult expected = null; // TODO: Initialize to an appropriate value
            AssignTagsToBoxCommandResult actual;
            actual = target.Execute(command);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
