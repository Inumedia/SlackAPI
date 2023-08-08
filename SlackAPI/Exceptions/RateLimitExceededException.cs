using System;

namespace SlackAPI.Exceptions
{
    public class RateLimitExceededException: Exception
    {
        public TimeSpan RetryIn { get; set; }
        public RateLimitExceededException(TimeSpan retryIn) : base("Rate limit exceeded")
        {
            RetryIn = retryIn;
        }
    }
}