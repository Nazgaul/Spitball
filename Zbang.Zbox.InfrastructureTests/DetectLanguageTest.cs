using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zbang.Zbox.Infrastructure;

namespace Zbang.Zbox.InfrastructureTests
{
    [TestClass]
    public class DetectLanguageTest
    {
        [TestMethod]
        public void DetectHebrewLanguage()
        {
            var detect = new DetectLanguage();
            var lang = detect.DoWork("היי מה קורה");
            Assert.AreEqual(lang, Infrastructure.Culture.Language.Hebrew);
        }

        [TestMethod]
        public void DetectEnglishLanguage()
        {
            var detect = new DetectLanguage();
            var lang = detect.DoWork("Hi my name is George");
            Assert.AreEqual(lang, Infrastructure.Culture.Language.EnglishUs);
        }
        [TestMethod]
        public void DetectEnglishUkLanguage()
        {
            var detect = new DetectLanguage();
            var lang = detect.DoWork("Hi the colour is red");
            Assert.AreEqual(lang, Infrastructure.Culture.Language.EnglishUs);
        }
        [TestMethod]
        public void DetectGermanLanguage()
        {
            var detect = new DetectLanguage();
            var lang = detect.DoWork("Wie geht's dir/ Ihnen?");
            Assert.AreEqual(lang, Infrastructure.Culture.Language.Undefined);
        }
        //
    }
}
