﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;

namespace SlackAPI
{
    class RequestState<K>
        where K : Response
    {
        public HttpWebRequest request;
        public HttpWebResponse response;
        public bool Success;

        Tuple<string, string>[] Post;
        Action<K> callback;

        public RequestState(HttpWebRequest requestData, Tuple<string, string>[] postParameters, Action<K> toCallback)
        {
            request = requestData;
            Post = postParameters;
            callback = toCallback;
        }

        public void Begin()
        {
            if (Post.Length == 0)
            {
                request.Method = "GET";
                IAsyncResult result = request.BeginGetResponse(GotResponse, this);
            }
            else
            {
                request.Method = "POST";
                IAsyncResult result = request.BeginGetRequestStream(GotRequest, this);
            }
        }

        public void GotRequest(IAsyncResult result)
        {
            if (result.AsyncState != this)
                throw new InvalidOperationException("This shouldn't be happening! D:");

            using (Stream requestStream = request.EndGetRequestStream(result))
                if (Post.Length > 0)
                    using (StreamWriter writer = new StreamWriter(requestStream))
                    {
                        bool first = true;
                        foreach (Tuple<string, string> postEntry in Post)
                        {
                            if (!first)
                                writer.Write(',');
                            
                            writer.Write(string.Format("{0}={1}", Uri.EscapeDataString(postEntry.Item1), Uri.EscapeDataString(postEntry.Item2)));

                            first = false;
                        }
                    }

            request.BeginGetResponse(GotResponse, this);
        }

        internal void GotResponse(IAsyncResult result)
        {
            try
            {
                response = (HttpWebResponse)request.EndGetResponse(result);
                Success = true;
            }
            catch (WebException we)
            {
                //Anything that doesn't return error 200 throws an exception.  Sucks.  :l
                response = (HttpWebResponse)we.Response;
                //TODO: Handle timeouts, etc?
            }

            K responseObj;

            using(Stream responseReading = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(responseReading))
            {
                string responseData = reader.ReadToEnd();
                responseObj = JsonConvert.DeserializeObject<K>(responseData, new JavascriptDateTimeConverter());
            }

            if(callback != null)
                callback(responseObj);
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited=false)]
    public class RequestPath : Attribute
    {
        //See notes in Slack:APIRequest<K>
        public string Path;
        public bool UsePrimaryAPI;
        public RequestPath(string requestPath, bool isPrimaryAPI = true)
        {
            Path = requestPath;
            UsePrimaryAPI = isPrimaryAPI;
        }

        static Dictionary<Type, RequestPath> paths = new Dictionary<Type, RequestPath>();

        public static RequestPath GetRequestPath<K>()
        {
            Type t = typeof(K);
            if (paths.ContainsKey(t))
                return paths[t];

            TypeInfo info = t.GetTypeInfo();

            RequestPath path = info.GetCustomAttribute<RequestPath>();
            if (path == null) throw new InvalidOperationException(string.Format("No valid request path for {0}", t.Name));

            paths.Add(t, path);
            return path;
        }
    }
}
