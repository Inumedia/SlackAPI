using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    public abstract class Response
    {
        /// <summary>
        /// Should always be checked before trying to process a response.
        /// </summary>
        public bool ok;

        /// <summary>
        /// Purely speculative. Might not be bools, and might not always be included when a request fails.
        /// </summary>
        public bool invalid_auth;
        /// <summary>
        /// Purely speculative. Might not be bools, and might not always be included when a request fails.
        /// </summary>
        public bool account_inactive;
    }
}
