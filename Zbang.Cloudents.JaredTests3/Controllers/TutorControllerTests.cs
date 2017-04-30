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
        public void GetTestSubject()
        {
            var controller = new TutorController();
            var first = controller.Get("Medical Studies");
            var second = controller.Get("Medical studies");
            var third = controller.Get("medical Studies");
            Console.WriteLine(first.ElementAt(13).Name);
            Console.WriteLine(second.ElementAt(13).Name);
            Console.WriteLine(third.ElementAt(13).Name);
            Console.WriteLine(third.ElementAt(12).Name);
            Console.WriteLine(second.ElementAt(12).Name);
            Console.WriteLine(first.ElementAt(12).Name);
            Console.WriteLine(first.Count());
            Console.WriteLine(second.Count());
            Console.WriteLine(third.Count());
            //Assert.IsTrue(first.Count() != second.Count());
            Assert.IsTrue(((!first.ElementAt(13).Name.Equals(second.ElementAt(13).Name))|| (!(first.ElementAt(13).Name.Equals(third.ElementAt(13).Name)))));
        }
        [TestMethod()]
        public void GetTestNoMatch()
        {
            var controller = new TutorController();
            var first = controller.Get("mathuih");
            var second = controller.Get("mahuiiith");
            var third = controller.Get("maith");
            Console.WriteLine(first.ElementAt(13).Name);
            Console.WriteLine(second.ElementAt(13).Name);
            Console.WriteLine(third.ElementAt(13).Name);
            Console.WriteLine(third.ElementAt(12).Name);
            Console.WriteLine(second.ElementAt(12).Name);
            Console.WriteLine(first.ElementAt(12).Name);
            Console.WriteLine(first.Count());
            Console.WriteLine(second.Count());
            Console.WriteLine(third.Count());
            //Assert.IsTrue(first.Count() != second.Count());
            Assert.IsTrue(((!first.ElementAt(13).Name.Equals(second.ElementAt(13).Name)) || (!(first.ElementAt(13).Name.Equals(third.ElementAt(13).Name)))));
        }
        [TestMethod()]
        public void GetTestKeyWords()
        {
            var controller = new TutorController();
            var first = controller.Get("calculus");
            var second = controller.Get("calculus");
            var third = controller.Get("Calculus");
            Console.WriteLine(first.ElementAt(13).Name);
            Console.WriteLine(second.ElementAt(13).Name);
            Console.WriteLine(third.ElementAt(13).Name);
            Console.WriteLine(third.ElementAt(12).Name);
            Console.WriteLine(second.ElementAt(12).Name);
            Console.WriteLine(first.ElementAt(12).Name);
            Console.WriteLine(first.Count());
            Console.WriteLine(second.Count());
            Console.WriteLine(third.Count());
            //Assert.IsTrue(first.Count() != second.Count());
            Assert.IsTrue(((!first.ElementAt(13).Name.Equals(second.ElementAt(13).Name)) || (!(first.ElementAt(13).Name.Equals(third.ElementAt(13).Name)))));
        }
    }
}