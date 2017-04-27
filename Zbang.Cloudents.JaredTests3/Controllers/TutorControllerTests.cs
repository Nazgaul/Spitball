using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zbang.Cloudents.Jared.Controllers;

namespace Zbang.Cloudents.JaredTests3.Controllers
{
    [TestClass()]
    public class TutorControllerTests
    {
        [TestMethod()]
        public void GetTest1()
        {
            var controller = new TutorController();
            var first = controller.Get("math");
            var second = controller.Get("math");
            var third = controller.Get("math");
            Console.WriteLine(first.ElementAt(13).Name);
            Console.WriteLine(second.ElementAt(13).Name);
            Console.WriteLine(third.ElementAt(13).Name);
            Console.WriteLine(first.Count());
            Console.WriteLine(second.Count());
            Console.WriteLine(third.Count());
            //Assert.IsTrue(first.Count() != second.Count());
            Assert.IsTrue((!first.ElementAt(13).Name.Equals(second.ElementAt(13).Name))|| (!first.ElementAt(13).Name.Equals(third.ElementAt(13).Name)));
        }
    }
}