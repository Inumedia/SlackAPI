using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SlackAPI
{
    /// <summary>
    /// Used to configure proxy settings
    /// </summary>
    public class Proxy
    {
        public WebProxy ProxySettings { get; }

        public Proxy(string url)
        {
            ProxySettings = new WebProxy
            {
                Address = new Uri(url)
            };
        }

        public Proxy(string url, string username, string password)
        {
            ProxySettings = new WebProxy
            {
                Address = new Uri(url),
                Credentials = new NetworkCredential(username, password)
            };
        }

        public Proxy(WebProxy proxy)
        {
            ProxySettings = proxy;
        }
    }
}
