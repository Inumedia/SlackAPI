﻿using Newtonsoft.Json;
using SlackAPI.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#if NETSTANDARD1_6
using Microsoft.Extensions.DependencyModel;
#endif

namespace SlackAPI
{
    public class SlackSocket
    {
        LockFreeQueue<string> sendingQueue;
        int currentlySending;
        int closedEmitted;
        CancellationTokenSource cts;

        Dictionary<int, Action<string>> callbacks;
        internal ClientWebSocket socket;
        int currentId;

        Dictionary<string, Dictionary<string, Delegate>> routes;
        public bool Connected { get { return socket != null && socket.State == WebSocketState.Open; } }
        public event Action<WebSocketException> ErrorSending;
        public event Action<WebSocketException> ErrorReceiving;
        public event Action<Exception> ErrorReceivingDesiralization;
        public event Action<Exception> ErrorHandlingMessage;
        public event Action ConnectionClosed;

        //This would be done for hinting but I don't think we really need this.

        static Dictionary<string, Dictionary<string, Type>> routing;
        static SlackSocket()
        {
            routing = new Dictionary<string, Dictionary<string, Type>>();
#if NET45
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.GlobalAssemblyCache == false);
#elif NETSTANDARD1_6
             var assemblies = DependencyContext.Default.GetDefaultAssemblyNames().Select(Assembly.Load);
#elif NETSTANDARD1_3
            var assemblies = new[] { typeof(SlackSocket).GetType().GetTypeInfo().Assembly };
#warning Autodetection not supported
#else
#error Platform not supported
#endif
            foreach (var assembly in assemblies)
            {
                Type[] assemblyTypes;
                try
                {
                    assemblyTypes = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException)
                {
                    return;
                }

                foreach (var type in assemblyTypes)
                {
                    foreach (var route in type.GetTypeInfo().GetCustomAttributes<SlackSocketRouting>())
                    {
                        if (!routing.ContainsKey(route.Type))
                        {
                            routing.Add(route.Type, new Dictionary<string, Type>()
                            {
                                {route.SubType ?? "null", type}
                            });
                        }
                        else
                        {
                            if (!routing[route.Type].ContainsKey(route.SubType ?? "null"))
                            {
                                routing[route.Type].Add(route.SubType ?? "null", type);
                            }
                            else
                            {
                                throw new InvalidProgramException("Cannot have two socket message types with the same type and subtype!");
                            }
                        }
                    }
                }
            }
        }

        public SlackSocket(LoginResponse loginDetails, object routingTo, Action onConnected = null)
        {
            BuildRoutes(routingTo);
            socket = new ClientWebSocket();
            callbacks = new Dictionary<int, Action<string>>();
            sendingQueue = new LockFreeQueue<string>();
            currentId = 1;

            cts = new CancellationTokenSource();
            socket.ConnectAsync(new Uri(string.Format("{0}?svn_rev={1}&login_with_boot_data-0-{2}&on_login-0-{2}&connect-1-{2}", loginDetails.url, loginDetails.svn_rev, DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds)), cts.Token).Wait();
            if (onConnected != null)
                onConnected();
            SetupReceiving();
        }

        void BuildRoutes(object routingTo)
        {
            routes = new Dictionary<string, Dictionary<string, Delegate>>();

            var routingToType = routingTo.GetType();
            var slackMessage = typeof(SlackSocketMessage);
            foreach (var m in routingTo.GetType().GetMethods(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public))
            {
                var parameters = m.GetParameters();
                if (parameters.Length != 1) continue;
                if (parameters[0].ParameterType.GetTypeInfo().IsSubclassOf(slackMessage))
                {
                    var t = parameters[0].ParameterType;
                    foreach (var route in t.GetTypeInfo().GetCustomAttributes<SlackSocketRouting>())
                    {
                        var genericAction = typeof(Action<>).MakeGenericType(parameters[0].ParameterType);
                        var d = m.CreateDelegate(genericAction, routingTo);
                        if (d == null)
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format("Couldn't create delegate for {0}.{1}", routingToType.FullName, m.Name));
                            continue;
                        }
                        if (!routes.ContainsKey(route.Type))
                            routes.Add(route.Type, new Dictionary<string, Delegate>());
                        if (!routes[route.Type].ContainsKey(route.SubType ?? "null"))
                            routes[route.Type].Add(route.SubType ?? "null", d);
                        else
                            routes[route.Type][route.SubType ?? "null"] = Delegate.Combine(routes[route.Type][route.SubType ?? "null"], d);
                    }
                }
            }
        }

        public void Send<K>(SlackSocketMessage message, Action<K> callback)
            where K : SlackSocketMessage
        {
            var sendingId = Interlocked.Increment(ref currentId);
            message.id = sendingId;
            callbacks.Add(sendingId, (c) =>
            {
                var obj = c.Deserialize<K>();
                callback(obj);
            });
            Send(message);
        }

        public void Send(SlackSocketMessage message)
        {
            if (message.id == 0)
                message.id = Interlocked.Increment(ref currentId);
            //socket.Send(JsonConvert.SerializeObject(message));

            if (string.IsNullOrEmpty(message.type))
            {
                var routes = message.GetType().GetTypeInfo().GetCustomAttributes<SlackSocketRouting>();

                SlackSocketRouting route = null;
                foreach (var r in routes)
                {
                    route = r;
                }
                if (route == null) throw new InvalidProgramException("Cannot send without a proper route!");
                else
                {
                    message.type = route.Type;
                    message.subtype = route.SubType;
                }
            }

            sendingQueue.Push(JsonConvert.SerializeObject(message, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            if (Interlocked.CompareExchange(ref currentlySending, 1, 0) == 0)
                Task.Factory.StartNew(HandleSending);
        }

        public void BindCallback<K>(Action<K> callback)
        {
            var t = typeof(K);
            foreach (var route in t.GetTypeInfo().GetCustomAttributes<SlackSocketRouting>())
            {
                if (!routes.ContainsKey(route.Type))
                    routes.Add(route.Type, new Dictionary<string, Delegate>());
                if (!routes[route.Type].ContainsKey(route.SubType ?? "null"))
                    routes[route.Type].Add(route.SubType ?? "null", callback);
                else
                    routes[route.Type][route.SubType ?? "null"] = Delegate.Combine(routes[route.Type][route.SubType ?? "null"], callback);
            }
        }

        public void UnbindCallback<K>(Action<K> callback)
        {
            var t = typeof(K);
            foreach (var route in t.GetTypeInfo().GetCustomAttributes<SlackSocketRouting>())
            {
                var d = routes.ContainsKey(route.Type) ? (routes.ContainsKey(route.SubType ?? "null") ? routes[route.Type][route.SubType ?? "null"] : null) : null;
                if (d != null)
                {
                    var newd = Delegate.Remove(d, callback);
                    routes[route.Type][route.SubType ?? "null"] = newd;
                }
            }
        }

        void SetupReceiving()
        {
            Task.Factory.StartNew(
                async () =>
                {
                    var buffers = new List<byte[]>();
                    var bytes = new byte[1024];
                    buffers.Add(bytes);
                    var buffer = new ArraySegment<byte>(bytes);
                    while (socket.State == WebSocketState.Open)
                    {
                        WebSocketReceiveResult result = null;
                        try
                        {
                            result = await socket.ReceiveAsync(buffer, cts.Token);
                        }
                        catch (WebSocketException wex)
                        {
                            if (ErrorReceiving != null)
                                ErrorReceiving(wex);
                            Close();
                            break;
                        }

                        if (!result.EndOfMessage && buffer.Count == buffer.Array.Length)
                        {
                            bytes = new byte[1024];
                            buffers.Add(bytes);
                            buffer = new ArraySegment<byte>(bytes);
                            continue;
                        }

                        var data = string.Join("", buffers.Select((c) => Encoding.UTF8.GetString(c).TrimEnd('\0')));
                        //Console.WriteLine("SlackSocket data = " + data);
                        SlackSocketMessage message = null;
                        try
                        {
                            message = data.Deserialize<SlackSocketMessage>();
                        }
                        catch (JsonException jsonExcep)
                        {
                            if (ErrorReceivingDesiralization != null)
                                ErrorReceivingDesiralization(jsonExcep);
                            continue;
                        }

                        if (message == null)
                            continue;
                        else
                        {
                            HandleMessage(message, data);
                            buffers = new List<byte[]>();
                            bytes = new byte[1024];
                            buffers.Add(bytes);
                            buffer = new ArraySegment<byte>(bytes);
                        }
                    }
                }, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        void HandleMessage(SlackSocketMessage message, string data)
        {
            if (callbacks.ContainsKey(message.reply_to))
                callbacks[message.reply_to](data);
            else if (routes.ContainsKey(message.type) && routes[message.type].ContainsKey(message.subtype ?? "null"))
            {
                try
                {
                    object o = null;
                    if (routing.ContainsKey(message.type) &&
                        routing[message.type].ContainsKey(message.subtype ?? "null"))
                        o = data.Deserialize(routing[message.type][message.subtype ?? "null"]);
                    else
                    {
                        //I believe this method is slower than the former. If I'm wrong we can just use this instead. :D
                        var t = routes[message.type][message.subtype ?? "null"].GetMethodInfo().GetParameters()[0].ParameterType;
                        o = data.Deserialize(t);
                    }
                    routes[message.type][message.subtype ?? "null"].DynamicInvoke(o);
                }
                catch (Exception e)
                {
                    if (ErrorHandlingMessage != null)
                        ErrorHandlingMessage(e);
                    throw e;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(string.Format("No valid route for {0} - {1}", message.type, message.subtype ?? "null"));
                if (ErrorHandlingMessage != null)
                    ErrorHandlingMessage(new InvalidDataException(string.Format("No valid route for {0} - {1}", message.type, message.subtype ?? "null")));
            }
        }

        void HandleSending()
        {
            string message;
            while (sendingQueue.Pop(out message) && socket.State == WebSocketState.Open && !cts.Token.IsCancellationRequested)
            {
                var sending = Encoding.UTF8.GetBytes(message);
                var buffer = new ArraySegment<byte>(sending);
                try
                {
                    socket.SendAsync(buffer, WebSocketMessageType.Text, true, cts.Token).Wait();
                }
                catch (WebSocketException wex)
                {
                    if (ErrorSending != null)
                        ErrorSending(wex);
                    Close();
                    break;
                }
            }

            currentlySending = 0;
        }

        public void Close()
        {
            try
            {
                this.socket.Abort();
            }
            catch (Exception ex)
            {
            }

            if (Interlocked.CompareExchange(ref closedEmitted, 1, 0) == 0 && ConnectionClosed != null)
                ConnectionClosed();
        }
    }

    public class SlackSocketMessage
    {
        public int id;
        public int reply_to;
        public string type;
        public string subtype;
        public bool ok = true;
        public Error error;
    }

    public class Error
    {
        public int code;
        public string msg;
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class SlackSocketRouting : Attribute
    {
        public string Type;
        public string SubType;
        public SlackSocketRouting(string type, string subtype = null)
        {
            this.Type = type;
            this.SubType = subtype;
        }
    }
}