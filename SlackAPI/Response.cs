using System;

namespace SlackAPI
{
    public abstract class Response
    {
        /// <summary>
        /// Should always be checked before trying to process a response.
        /// </summary>
        public bool ok;

        /// <summary>
        /// if ok is false, then this is the reason-code
        /// </summary>
        public string error;

        public void AssertOk()
        {
            if (!(ok))
                throw new InvalidOperationException($"An error occurred: {this.error}");
        }

        public ResponseMetadata response_metadata;
    }

    public class ResponseMetadata
    {
	    public string next_cursor;
    }
}