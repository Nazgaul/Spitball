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
    ///This is a test class for CreateMembershipUserCommandHandlerTest and is intended
    ///to contain all CreateMembershipUserCommandHandlerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CreateMembershipUserCommandHandlerTest
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
        ///A test for CreateMembershipUserCommandHandler Constructor
        ///</summary>
        [TestMethod()]
        public void CreateMembershipUserCommandHandlerConstructorTest()
        {
            IUserRepository userRepository = null; // TODO: Initialize to an appropriate value
            IRepository<Box> boxRepository = null; // TODO: Initialize to an appropriate value
            IRepository<UserBoxRel> userBoxRelationshipRepository = null; // TODO: Initialize to an appropriate value
            CreateMembershipUserCommandHandler target = new CreateMembershipUserCommandHandler(userRepository, boxRepository, userBoxRelationshipRepository);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Execute
        ///</summary>
        [TestMethod()]
        public void ExecuteTest()
        {
            IUserRepository userRepository = null; // TODO: Initialize to an appropriate value
            IRepository<Box> boxRepository = null; // TODO: Initialize to an appropriate value
            IRepository<UserBoxRel> userBoxRelationshipRepository = null; // TODO: Initialize to an appropriate value
            CreateMembershipUserCommandHandler target = new CreateMembershipUserCommandHandler(userRepository, boxRepository, userBoxRelationshipRepository); // TODO: Initialize to an appropriate value
            CreateUserCommand command = null; // TODO: Initialize to an appropriate value
            CreateUserCommandResult expected = null; // TODO: Initialize to an appropriate value
            CreateUserCommandResult actual;
            actual = target.Execute(command);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
