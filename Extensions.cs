using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    public static class Extensions
    {
        /// <summary>
        /// Converts to a propert JavaScript timestamp interpretted by Slack.  Also handles converting to UTC.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public static string ToProperTimeStamp(this DateTime that, bool toUTC = true)
        {
            if (toUTC)
                return that.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString();
            else
                return that.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString();
        }
    }
}
