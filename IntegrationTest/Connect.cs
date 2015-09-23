using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlackAPI;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Newtonsoft.Json;

namespace IntegrationTest
{
    [TestClass]
    public class Connect
    {
        string testText = "Test :D";
        string testChannel = "SuperSecretChannel";
        string token = "token-tokentoken-tokentoken-tokentoken-token";
        [TestMethod]
        public void TestConnect()
        {
            EventWaitHandle wait = new EventWaitHandle(false, EventResetMode.ManualReset);

            SlackSocketClient client = new SlackSocketClient(token);
            client.Connect((o) =>
            {
                Debug.WriteLine("RTM Start");
            }, () =>
            {
                Debug.WriteLine("Connected");
                wait.Set();
            });

            Assert.IsTrue(wait.WaitOne(10000), "Didn't return within a reasonable time.");
            Thread.Sleep(500);
            Assert.IsTrue(client.IsConnected, "Invalid, doesn't think it's connected.");
        }

        [TestMethod]
        public void TestConnectPostAndDelete()
        {
            EventWaitHandle wait = new EventWaitHandle(false, EventResetMode.ManualReset);

            SlackSocketClient client = new SlackSocketClient(token);
            client.Connect((o) =>
            {
                Debug.WriteLine("RTM Start");
            }, () =>
            {
                Debug.WriteLine("Connected");
                wait.Set();
            });

            Assert.IsTrue(wait.WaitOne(10000), "Didn't return within a reasonable time.");
            Thread.Sleep(500);
            Assert.IsTrue(client.IsConnected, "Invalid, doesn't think it's connected.");

            wait = new EventWaitHandle(false, EventResetMode.ManualReset);
            DateTime ts = DateTime.MinValue;
            SlackAPI.WebSocketMessages.MessageReceived a = null;
            client.SendMessage((resp) =>
            {
                a = resp;
                wait.Set();
            }, testChannel, testText);

            Assert.IsTrue(wait.WaitOne(1000), "Took too long for Slack to acknowledge message.");
            
            ts = a.ts;
            Assert.AreEqual(a.text, testText, "Got invalid returned text, something's not right here...");

            DeletedResponse r = null;
            wait = new EventWaitHandle(false, EventResetMode.ManualReset);
            client.DeleteMessage((resp) =>
            {
                r = resp;
                wait.Set();
            }, testChannel, ts);

            Assert.IsTrue(wait.WaitOne(1000), "Took too long for Slack to acknowledge delete.");
            Assert.IsTrue(r.ok, "Message not deleted!");
            Assert.AreEqual(r.channel, testChannel, "Got invalid channel? Something's not right here...");
            Assert.AreEqual(r.ts, ts, "Got invalid time stamp? Something's not right here...");
        }

        [TestMethod]
        public void TestConnectBot()
        {
            EventWaitHandle wait = new EventWaitHandle(false, EventResetMode.ManualReset);

            SlackSocketClient client = new SlackSocketClient("BotsTotallySupportRTM:)");
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
