using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zbang.Zbox.Domain.Common;

namespace Zbang.Zbox.Domain.CommonTests
{
    [TestClass]
    public class TextManipulationTests
    {
        [TestMethod]
        public void EncodeText_SomeHtmlFormat_EncodeProperly()
        {
            const string SomeHtmlText= "<p>dfas<b>dfsdfdfgdfgsdfsdf</b>sdfsfsdfsd<i>sdfsdfsdf<u>sdasdasdasd<font size=\"4\">sdfsfsdfsdf</font></u></i></p><p></p><ol><li><font size=\"4\"><i><u>dasdasd</u></i></font></li><li><font size=\"4\"><i><u>sdfsdfsd</u></i></font></li></ol><p></p>";
           var result =  TextManipulation.EncodeText(SomeHtmlText, Question.AllowedHtmlTag);

           Assert.AreEqual(SomeHtmlText, result);
        }


        [TestMethod]
        public void EncodeText_SomeHtmlNotValidFormat_EncodeProperly()
        {
            const string SomeHtmlText = "<script>alert('h')</script><p>dfas<b>dfsdfdfgdfgsdfsdf</b>sdfsfsdfsd<i>sdfsdfsdf<u>sdasdasdasd<font size=\"4\">sdfsfsdfsdf</font></u></i></p><p></p><ol><li><font size=\"4\"><i><u>dasdasd</u></i></font></li><li><font size=\"4\"><i><u>sdfsdfsd</u></i></font></li></ol><p></p>";
            var result = TextManipulation.EncodeText(SomeHtmlText, Question.AllowedHtmlTag);

            Assert.AreNotEqual(SomeHtmlText, result);
        }
    }
}
