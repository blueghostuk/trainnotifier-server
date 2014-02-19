using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrainNotifier.Common.Model;
using TrainNotifier.Common.Model.SmartExtract;

namespace UnitTests
{
    [TestClass]
    public class TDElementTest
    {
        [TestMethod]
        public void TestEquals()
        {
            CcTD ccTd = new CcTD
            {
                AreaId = "BN",
                Description = "2R94",
                Time = DateTime.UtcNow,
                To = "COUT",
                Type = "CC"
            };

            TDElement nonMatchingElement = new TDElement
            {
                TD = "BN",
                FROMBERTH = "123",
                TOBERTH = "COUT"
            };

            TDElement matchingElement = new TDElement
            {
                TD = "BN",
                FROMBERTH = string.Empty,
                TOBERTH = "COUT"
            };

            Assert.IsFalse(nonMatchingElement.Equals(ccTd));
            Assert.IsTrue(matchingElement.Equals(ccTd));
        }
    }
}
