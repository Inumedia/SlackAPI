using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlackAPI;
using System;
using System.Threading;

namespace IntegrationTest
{
    [TestClass]
    public class Connect
    {
        [TestMethod]
        public void TestConnect()
        {
            EventWaitHandle wait = new EventWaitHandle(false, EventResetMode.ManualReset);

            SlackSocketClient client = new SlackSocketClient("AUTH_TOKEN");
            client.Connect((c) =>
            {
                wait.Set();
            });

            Assert.IsTrue(wait.WaitOne(1000) || client.IsConnected);

            Console.WriteLine("Wooo");
        }
    }
}
