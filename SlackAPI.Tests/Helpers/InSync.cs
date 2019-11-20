using System;
using System.Diagnostics;
using System.Threading;
using System.Runtime.CompilerServices;
using Xunit;

namespace SlackAPI.Tests.Helpers
{
    public class InSync : IDisposable
    {
        private readonly TimeSpan DefaultWaitTimeout = TimeSpan.FromSeconds(15);

        private readonly ManualResetEventSlim waiter;
        private readonly string message;
        private readonly TimeSpan waitTimeout;

        public InSync([CallerMemberName] string message = null, TimeSpan? waitTimeout = null)
        {
            this.message = message;
            this.waitTimeout = waitTimeout.GetValueOrDefault(DefaultWaitTimeout);
            this.waiter = new ManualResetEventSlim();
        }

        public void Proceed()
        {
            this.waiter.Set();
        }

        public void Dispose()
        {
            Assert.True(this.waiter.Wait(Debugger.IsAttached ? Timeout.InfiniteTimeSpan : this.waitTimeout), $"Took too long to do '{this.message}'");
        }
    }
}