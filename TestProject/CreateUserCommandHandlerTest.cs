using Zbang.Zbox.Domain.CommandHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain;

namespace TestProject
{
    
    
    /// <summary>
    ///This is a test class for CreateUserCommandHandlerTest and is intended
    ///to contain all CreateUserCommandHandlerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CreateUserCommandHandlerTest
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


        internal virtual CreateUserCommandHandler CreateCreateUserCommandHandler()
        {
            // TODO: Instantiate an appropriate concrete class.
            CreateUserCommandHandler target = null;
            return target;
        }

        /// <summary>
        ///A test for Execute
        ///</summary>
        [TestMethod()]
        public void ExecuteTest()
        {
            CreateUserCommandHandler target = CreateCreateUserCommandHandler(); // TODO: Initialize to an appropriate value
            CreateUserCommand command = null; // TODO: Initialize to an appropriate value
            CreateUserCommandResult expected = null; // TODO: Initialize to an appropriate value
            CreateUserCommandResult actual;
            actual = target.Execute(command);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        internal virtual CreateUserCommandHandler_Accessor CreateCreateUserCommandHandler_Accessor()
        {
            // TODO: Instantiate an appropriate concrete class.
            CreateUserCommandHandler_Accessor target = null;
            return target;
        }

        /// <summary>
        ///A test for GenerateDefaultBox
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Zbang.Zbox.Domain.CommandHandlers.dll")]
        public void GenerateDefaultBoxTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            CreateUserCommandHandler_Accessor target = new CreateUserCommandHandler_Accessor(param0); // TODO: Initialize to an appropriate value
            User user = null; // TODO: Initialize to an appropriate value
            target.GenerateDefaultBox(user);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
    }
}
