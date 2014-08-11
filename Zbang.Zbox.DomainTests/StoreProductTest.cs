using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zbang.Zbox.Domain;

namespace Zbang.Zbox.DomainTests
{
    [TestClass]
    public class StoreProductTest
    {
        [TestMethod]
        public void BuildProduct_CheckFeaturedPrice()
        {
            var features = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("שדרוג כרטיס זכרון",
                    "8GB SDHC CLASS10 *29*, 16GB SDHC CLASS10 *49*, 32GB SDHC CLASS10 *119*")
            };
            var storeProduct = new StoreProduct(0, "some name",
                "extra",
                0,
                0,
                0,
                "some url",
                null,
                "desc",
                false,
                "supply",
                0,
                "catalog",
                0,
                "producer",
                features, 0, 0, 1, 15);

            Assert.AreEqual(29, storeProduct.FeaturesReadOnly.FirstOrDefault().Price);
        }


        [TestMethod]
        public void BuildProduct_CheckFeaturedDescription()
        {
            var features = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("שדרוג כרטיס זכרון",
                    "8GB SDHC CLASS10 *29*, 16GB SDHC CLASS10 *49*, 32GB SDHC CLASS10 *119*")
            };
            var storeProduct = new StoreProduct(0, "some name",
                "extra",
                0,
                0,
                0,
                "some url",
                null,
                "desc",
                false,
                "supply",
                0,
                "catalog",
                0,
                "producer",
                features, 0, 0, 1, 15);

            Assert.AreEqual("8GB SDHC CLASS10", storeProduct.FeaturesReadOnly.FirstOrDefault().Description);
        }
    }
}
