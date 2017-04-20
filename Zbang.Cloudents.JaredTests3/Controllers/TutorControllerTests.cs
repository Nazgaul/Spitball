using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zbang.Cloudents.Jared.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Cloudents.Jared.Controllers.Tests
{
    [TestClass()]
    public class TutorControllerTests
    {
        [TestMethod()]
        public void GetTest()
        {
            new TutorController().Get("");
        }
    }
}