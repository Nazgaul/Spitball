using Zbang.Zbox.Domain.CommandHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Zbang.Zbox.Domain;
using Zbang.Zbox.Infrastructure.Repositories;
using Zbang.Zbox.Domain.Commands;

namespace TestProject
{
    
    
    /// <summary>
    ///This is a test class for UpdateThumbnailCommandHandlerTest and is intended
    ///to contain all UpdateThumbnailCommandHandlerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UpdateThumbnailCommandHandlerTest
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
        ///A test for UpdateThumbnailCommandHandler Constructor
        ///</summary>
        [TestMethod()]
        public void UpdateThumbnailCommandHandlerConstructorTest()
        {
            IRepository<File> itemRepository = null; // TODO: Initialize to an appropriate value
            UpdateThumbnailCommandHandler target = new UpdateThumbnailCommandHandler(itemRepository);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Execute
        ///</summary>
        [TestMethod()]
        public void ExecuteTest()
        {
            IRepository<File> itemRepository = null; // TODO: Initialize to an appropriate value
            UpdateThumbnailCommandHandler target = new UpdateThumbnailCommandHandler(itemRepository); // TODO: Initialize to an appropriate value
            UpdateThumbnailCommand command = null; // TODO: Initialize to an appropriate value
            UpdateThumbnailCommandResult expected = null; // TODO: Initialize to an appropriate value
            UpdateThumbnailCommandResult actual;
            actual = target.Execute(command);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
