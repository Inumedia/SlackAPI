using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
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
            if (requestData == null) throw new ArgumentNullException("requestData can not be null");
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
                response = (HttpWebResponse)request?.EndGetResponse(result);
                Success = true;
            }
            catch (WebException we)
            {
                // If we don't get a response, let the exception bubble up as we can't do anything
                if (we.Response == null)
                {
                    var defaultResponse = CreateDefaultResponseForError(we);
                    callback?.Invoke(defaultResponse);
                    return;
                }
                
                //Anything that doesn't return error 200 throws an exception.  Sucks.  :l
                response = (HttpWebResponse)we.Response;
                //TODO: Handle timeouts, etc?
            }
            catch (Exception e)
            {
                var defaultResponse = CreateDefaultResponseForError(e);
                callback?.Invoke(defaultResponse);
                return;
            }

            K responseObj;
            if (response == null)
            {
                responseObj = CreateDefaultResponseForError(new Exception("Empty response"));
                callback?.Invoke(responseObj);
                return;
            }

            try
            {
                using (Stream responseReading = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseReading))
                {
                    string responseData = reader.ReadToEnd();
                    responseObj = responseData.Deserialize<K>();
                }
            }
            catch (Exception e)
            {
                responseObj = CreateDefaultResponseForError(e);
            }

            callback?.Invoke(responseObj);
        }

        private K CreateDefaultResponseForError(Exception e)
        {
            var defaultResponse = (K)Activator.CreateInstance<K>();
            defaultResponse.ok = false;
            defaultResponse.error = e.ToString();
            return defaultResponse;
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

        static ConcurrentDictionary<Type, RequestPath> paths = new ConcurrentDictionary<Type, RequestPath>();

        public static RequestPath GetRequestPath<K>()
        {
            Type t = typeof(K);
            if (paths.TryGetValue(t, out var path))
                return path;

            TypeInfo info = t.GetTypeInfo();

            path = info.GetCustomAttribute<RequestPath>();
            if (path == null) throw new InvalidOperationException(string.Format("No valid request path for {0}", t.Name));

            try
            {
                // Some other thread may have already placed the path to the dictionary, 
                // the original one will remain there and current one will be GCed once it
                // gets out of scope of this request.
                paths.TryAdd(t, path);
            }
            catch (Exception e)
            {
                // There is a slight chance of TryAdd throwing, we want to be extra safe
                // so we just consume it and leave the dictionary as-is, next call of
                // the same function will try to add the path again.
                // This may be removed in the future if TryAdd is verified as safe.
                // See #190.
                Trace.TraceError(e.ToString());
            }

            return path;
        }
    }
}
