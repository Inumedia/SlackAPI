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
        /// if ok is false, then this is the reason-code
        /// </summary>
        public string error;

        /// <summary>
        /// May be supplied.
        /// </summary>
        public string needed;

        public void AssertOk()
        {
            if (!(ok))
            {
                if (error == "missing_scope")
                {
                    throw new InvalidOperationException(String.Format("missing_scope: needed: {0}", needed));
                }
                throw new InvalidOperationException(string.Format("An error occurred: {0}", this.error));
            }
        }
    }
}
