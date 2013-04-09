using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.IO;
using TrainNotifier.Common.Model.PPM;
using System.Text;
using Newtonsoft.Json.Linq;

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
            Assert.IsNotNull(dataObject.RTPPMDataMsgV1.RTPPMData.OperatorPage);

            var ppmData = PPMJsonMapper.ParsePPMData(dataObject.RTPPMDataMsgV1.RTPPMData);

            Assert.IsNotNull(ppmData);

//            const string sql = @"
//                INSERT INTO [natrail].[dbo].[PPMSectors]
//                       ([OperatorCode]
//                       ,[SectorCode]
//                       ,[Description])
//                 VALUES
//                       ({0}
//                       ,'{1}'
//                       ,'{2}');";

//            StringBuilder sr = new StringBuilder();

//            foreach (var op in dataObject.RTPPMDataMsgV1.RTPPMData.OperatorPage)
//            {
//                string code = op.Operator.code;
//                if (op.OprServiceGrp != null)
//                {
//                    if (op.OprServiceGrp is JArray)
//                    {
//                        foreach (var sg in op.OprServiceGrp)
//                        {
//                            sr.AppendFormat(sql, code, sg.sectorCode, sg.name);
//                            sr.AppendLine();
//                        }
//                    }
//                    else
//                    {
//                        sr.AppendFormat(sql, code, op.OprServiceGrp.sectorCode, op.OprServiceGrp.name);
//                        sr.AppendLine();
//                    }
//                }
//            }

//            var sqlOp = sr.ToString();
        }

    }
}
