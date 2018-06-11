﻿using System;

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
                throw new InvalidOperationException(string.Format("An error occurred: {0}", this.error));
        }
    }
}
