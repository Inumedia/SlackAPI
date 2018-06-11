﻿using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace SlackAPI
{
    public class RequestStateForTask<K>
        where K : Response
    {
        public HttpWebRequest request;
        Tuple<string, string>[] Post;
        public bool Success;

        public RequestStateForTask(HttpWebRequest requestData, Tuple<string, string>[] postParameters)
        {
            request = requestData;
            Post = postParameters;
        }

        internal Task<K> Execute()
        {
            if (Post == null || Post.Length == 0)
            {
                request.Method = "GET";
                return this.ExecuteResult();
            }
            else
            {
                return this.ExecutePost();
            }
        }

        private async Task<K> ExecuteResult()
        {
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)await this.request.GetResponseAsync();
                Success = true;
            }
            catch (WebException we)
            {
                //Anything that doesn't return error 200 throws an exception.  Sucks.  :l
                response = (HttpWebResponse)we.Response;
                //TODO: Handle timeouts, etc?
            }

            K responseObj;

            using (var responseReading = response.GetResponseStream())
            {
                using (var reader = new StreamReader(responseReading))
                {
                    string responseData = reader.ReadToEnd();
                    responseObj = responseData.Deserialize<K>();
                }
            }

            return responseObj;
        }

        private async Task<K> ExecutePost()
        {
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            using (var requestStream = await request.GetRequestStreamAsync())
            {
                if (Post.Length > 0)
                {
                    using (var writer = new StreamWriter(requestStream))
                    {
                        var first = true;
                        foreach (var postEntry in Post)
                        {
                            if (!first)
                                writer.Write('&');

                            await writer.WriteAsync(string.Format("{0}={1}", Uri.EscapeDataString(postEntry.Item1), Uri.EscapeDataString(postEntry.Item2)));

                            first = false;
                        }
                    }
                }
            }

            return await this.ExecuteResult();
        }
    }
}
