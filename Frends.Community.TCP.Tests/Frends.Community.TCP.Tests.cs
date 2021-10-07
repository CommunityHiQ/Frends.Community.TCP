using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Threading;

namespace Frends.Community.TCP.Tests
{
  //  [Ignore("Test locally")]

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
                Commands = new Command[] { new Command { CommandString = "COMMAND1", ResponseStart = "<C", ResponseEnd = ">" },
                new Command{CommandString = "COMMAND2", ResponseStart = "<", ResponseEnd = "" },
                new Command{CommandString = "COMMAND3", ResponseStart = "<", ResponseEnd = ">" }},
                IpAddress = "127.0.0.1",
                Port = 13000
            };

            var options = new Options
            {
                Timeout = 60000,

            };

            var input2 = new Parameters
            {
                Commands = new Command[] { new Command { CommandString = "COMMAND1" }, new Command { CommandString = "" },
                    new Command {CommandString = "SEND_EMPTY_RESP" }, new Command {CommandString ="STOP" } },
                IpAddress = "127.0.0.1",
                Port = 13000
            };

            var options2 = new Options
            {
                
            };

            var res1 = TCPTasks.ASCIIRequest(input, options, token).Result.Responses;
            JArray expected = JArray.Parse(@"['<COMMAND1Response_ResponseContinues>','<COMMAND2Response','<COMMAND3Response_ResponseContinues>' ]");
            Assert.AreEqual(expected.ToString(), res1.ToString());
            Assert.That(async () => await TCPTasks.ASCIIRequest(input2, options2, token), Throws.Exception);
            
        }

        [Test]
        public void TestLatinData()
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            var input = new Parameters
            {
                Commands = new Command[] { new Command { CommandString = "²", ResponseStart = "*", ResponseEnd = "" }},
                IpAddress = "127.0.0.1",
                Port = 13000
            };

            var options = new Options
            {
                Timeout = 60000,

            };
            var res1 = TCPTasks.ASCIIRequest(input, options, token).Result.Responses;
            JArray expected = JArray.Parse(@"['*³']");
            Assert.AreEqual(expected.ToString(), res1.ToString());
        }

    }
}
