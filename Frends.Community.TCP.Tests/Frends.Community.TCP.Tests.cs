using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;

namespace Frends.Community.TCP.Tests
{
    [Ignore("Test locally with listener")]

    [TestFixture]
    class TestClass
    {
        [Test]
        public void TestSendMessage()
        {

            var input = new Parameters
            {
                Command = new string[] { "COMMAND1", "COMMAND2" },
                IpAddress = "127.0.0.1",
                Port = 13000
            };

            var options = new Options
            {
                Timeout = 15
            };

            var res1 = TCPTasks.ASCIIRequest(input, options).Result.Responses;
            JArray expected = JArray.Parse(@"['COMMAND1Response','COMMAND2Response']");
            Assert.AreEqual(expected, res1);

        }
    }
}
