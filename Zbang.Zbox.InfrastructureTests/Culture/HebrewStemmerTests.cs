using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zbang.Zbox.Infrastructure.Culture;

namespace Zbang.Zbox.InfrastructureTests.Culture
{
    [TestClass]
    public class HebrewStemmerTests
    {

        [TestMethod]
        public void StemAHebrewWord_LetterStartWithHeight_ReturnWithoutHeigh()
        {
            var word = "המכללה";

            var stemmer = new HebrewStemmer();
           var result =stemmer.StemAHebrewWord(word);

           Assert.AreEqual(result, "מכללה" + "%");


        }

        [TestMethod]
        public void StemAHebrewWord_PhraseStartWithHeight_ReturnWithoutHeigh()
        {
            var word = "המכללה האקדמית";

            var stemmer = new HebrewStemmer();
            var result = stemmer.StemAHebrewWord(word);

            Assert.AreEqual(result, "מכללה אקדמית" + "%");


        }
    }
}
