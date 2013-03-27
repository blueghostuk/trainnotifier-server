using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.IO;

namespace UnitTests
{
    [TestClass]
    public class RTTPMDataTest
    {
        [TestMethod]
        public void TestLoadFile()
        {
            const string fileName = "TestFiles\\RTPPMData.json";
            Assert.IsTrue(File.Exists(fileName));
            string data = File.ReadAllText(fileName);
            dynamic dataObject = JsonConvert.DeserializeObject<dynamic>(data);

            Assert.IsNotNull(dataObject.RTPPMDataMsgV1);
            Assert.IsNotNull(dataObject.RTPPMDataMsgV1.RTPPMData);
        }

    }
}
