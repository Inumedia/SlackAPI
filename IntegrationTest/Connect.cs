using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlackAPI;
using System;
using System.Diagnostics;
using System.IO;
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

            SlackSocketClient client = new SlackSocketClient("USER_TOKEN");
            client.Connect((o) =>
            {
                Debug.WriteLine("RTM Start");
            }, () =>
            {
                Debug.WriteLine("Connected");
                wait.Set();
            });

            Assert.IsTrue(wait.WaitOne(10000), "Didn't return within a reasonable time.");
            Assert.IsTrue(client.IsConnected, "Invalid, doesn't think it's connected.");
        }

        [TestMethod]
        public void TestConnectBot()
        {
            EventWaitHandle wait = new EventWaitHandle(false, EventResetMode.ManualReset);

            SlackSocketClient client = new SlackSocketClient("BOT_TOKEN");
            client.Connect((o) =>
            {
                Debug.WriteLine("RTM Start");
            }, () =>
            {
                Debug.WriteLine("Connected");
                wait.Set();
            });

            Assert.IsTrue(wait.WaitOne(10000), "Didn't return within a reasonable time.");
            Assert.IsTrue(client.IsConnected, "Invalid, doesn't think it's connected.");
        }
    }
}
