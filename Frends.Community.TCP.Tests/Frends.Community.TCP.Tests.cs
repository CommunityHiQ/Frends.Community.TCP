using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Threading;

namespace Frends.Community.TCP.Tests
{
    //[Ignore("Test locally")]

    [TestFixture]
    class TestClass
    {

        [OneTimeSetUp]
        public void StartListener()
        {

            Thread listenerThread = new Thread(new ThreadStart(TestListener.Listener));

            listenerThread.Start();

        }

        [Test]
        public void TestSendAndReceive()
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            var input = new Parameters
            {
                Command = new string[] { "COMMAND1", "COMMAND2"},
                IpAddress = "127.0.0.1",
                Port = 13000
            };

            var options = new Options
            {
                Timeout = 1000,
                ResponseStart = "<",
                ResponseEnd = ">"
            };

            var input2 = new Parameters
            {
                Command = new string[] { "COMMAND1", "COMMAND2", "", "SEND_EMPTY_RESP", "STOP" },
                IpAddress = "127.0.0.1",
                Port = 13000
            };

            var options2 = new Options
            {
                Timeout = 1000
            };
            var res1 = TCPTasks.ASCIIRequest(input, options, token).Result.Responses;
            JArray expected = JArray.Parse(@"['COMMAND1Response','COMMAND2Response']");
            Assert.AreEqual(expected.ToString(), res1.ToString()) ;
            Assert.That(async () => await TCPTasks.ASCIIRequest(input2, options2, token), Throws.Exception);          

        }

    }
}
