namespace IntegrationTest.Helpers
{
    using System;
    using System.Threading;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Polly;

    public class InSync : IDisposable
    {
        private readonly EventWaitHandle wait;

        public InSync()
        {
            wait = new EventWaitHandle(false, EventResetMode.ManualReset);
        }

        public void Proceed()
        {
            wait.Set();
        }

        public void Dispose()
        {
            Policy
                .Handle<AssertFailedException>()
                .WaitAndRetry(15, x => TimeSpan.FromSeconds(0.2), (exception, span) => Console.WriteLine("Retrying in {0} seconds", span.TotalSeconds))
                .Execute(() =>
                {
                    Assert.IsTrue(wait.WaitOne(), "Took too long to do the THING");
                });
        }
    }
}