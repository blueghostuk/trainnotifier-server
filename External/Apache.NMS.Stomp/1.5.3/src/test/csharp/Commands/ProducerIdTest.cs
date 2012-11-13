// /*
//  * Licensed to the Apache Software Foundation (ASF) under one or more
//  * contributor license agreements.  See the NOTICE file distributed with
//  * this work for additional information regarding copyright ownership.
//  * The ASF licenses this file to You under the Apache License, Version 2.0
//  * (the "License"); you may not use this file except in compliance with
//  * the License.  You may obtain a copy of the License at
//  *
//  *     http://www.apache.org/licenses/LICENSE-2.0
//  *
//  * Unless required by applicable law or agreed to in writing, software
//  * distributed under the License is distributed on an "AS IS" BASIS,
//  * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  * See the License for the specific language governing permissions and
//  * limitations under the License.
//  */
// 

using System;
using NUnit.Framework;
using Apache.NMS.Stomp.Commands;

namespace Apache.NMS.Stomp.Test.Commands
{
    [TestFixture]
    public class ProducerIdTest
    {
        [Test]
        public void TestAmqTypeProcessing()
        {
            ProducerId id = new ProducerId();
            id.ConnectionId = "cheese";
            id.SessionId = 2;
            id.Value = 3;

            string text = id.ToString();
            Assert.AreEqual("cheese:2:3", text, "ConsumerId as stomp");

            ProducerId another = new ProducerId("abc:5:6");
            Assert.AreEqual("abc", another.ConnectionId, "extracting consumerId.ConnectionId");
            Assert.AreEqual(5, another.SessionId, "extracting consumerId.SessionId");
            Assert.AreEqual(6, another.Value, "extracting consumerId.Value");
        }

        [Test]
        public void TestNonAmqTypeProcessing()
        {
            ProducerId id = new ProducerId();
            id.ConnectionId = "cheese";
            id.SessionId = 2;
            id.Value = 3;

            string text = id.ToString();
            Assert.AreEqual("cheese:2:3", text, "ConsumerId as stomp");

            ProducerId another = new ProducerId("abc56");
            Assert.AreEqual("abc56", another.ConnectionId, "extracting consumerId.ConnectionId");
            Assert.AreEqual(0, another.SessionId, "extracting consumerId.SessionId");
            Assert.AreEqual(0, another.Value, "extracting consumerId.Value");

            another = new ProducerId("abc:def");
            Assert.AreEqual("abc:def", another.ConnectionId, "extracting consumerId.ConnectionId");
            Assert.AreEqual(0, another.SessionId, "extracting consumerId.SessionId");
            Assert.AreEqual(0, another.Value, "extracting consumerId.Value");
        }
    }
}
