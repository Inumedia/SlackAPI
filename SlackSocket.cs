using System;
using System.Collections.Generic;
using System.Threading;
using WebSocket4Net;
using Newtonsoft.Json;
using System.Reflection;
using SlackAPI.Utilities;

namespace SlackAPI
{
    public class SlackSocket
    {
        LockFreeQueue<string> sendingQueue;
        int currentlySending;

        Dictionary<int, Action<string>> callbacks;
        WebSocket socket;
        int currentId;

        Dictionary<string, Dictionary<string, Delegate>> routes;
        public bool Connected { get { return socket != null && socket.State == WebSocketState.Open; } }

        //This would be done for hinting but I don't think we really need this.

        //static Dictionary<string, Dictionary<string, Type>> routing;
        /*static SlackSocket()
        {
            routing = new Dictionary<string, Dictionary<string, Type>>();
            foreach(Assembly assy in AppDomain.CurrentDomain.GetAssemblies())
                foreach(Type t in assy.GetTypes())
                    foreach(SlackSocketRouting route in t.GetCustomAttributes<SlackSocketRouting>())
                    {
                        if (!routing.ContainsKey(route.Type))
                            routing.Add(route.Type, new Dictionary<string, Type>()
                            {
                                {route.SubType, t}
                            });
                        else
                            if (!routing[route.Type].ContainsKey(route.SubType))
                                routing[route.Type].Add(route.SubType, t);
                            else
                                throw new InvalidProgramException("Cannot have two socket message types with the same type and subtype!");
                    }
        }*/

        public SlackSocket(LoginResponse loginDetails, object routingTo)
        {
            BuildRoutes(routingTo);
            socket = new WebSocket(string.Format("{0}?svn_rev={1}&login_with_boot_data-0-{2}&on_login-0-{2}&connect-1-{2}", loginDetails.url, loginDetails.svn_rev, DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds));
            callbacks = new Dictionary<int, Action<string>>();
            sendingQueue = new LockFreeQueue<string>();
            currentId = 1;
            socket.MessageReceived += socket_MessageReceived;

            socket.Open();

            //if (onConnected != null)
            //    onConnected(loginDetails);
        }

        void BuildRoutes(object routingTo)
        {
            routes = new Dictionary<string, Dictionary<string, Delegate>>();
            
            Type slackMessage = typeof(SlackSocketMessage);
            foreach (MethodInfo m in routingTo.GetType().GetMethods(BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public))
            {
                ParameterInfo[] parameters = m.GetParameters();
                if (parameters.Length != 1) continue;
                if(parameters[0].ParameterType.IsSubclassOf(slackMessage))
                {
                    Type t = parameters[0].ParameterType;
                    foreach (SlackSocketRouting route in t.GetCustomAttributes<SlackSocketRouting>())
                    {
                        Delegate d = new Action<string>((s) =>
                        {
                            object message = JsonConvert.DeserializeObject(s, t, new JavascriptDateTimeConverter());
                            m.Invoke(routingTo, new object[] { message });
                        });
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

        void socket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            string data = e.Message;
            SlackSocketMessage message = JsonConvert.DeserializeObject<SlackSocketMessage>(data, new JavascriptDateTimeConverter());

            if (message.reply_to.HasValue && callbacks.ContainsKey(message.reply_to.Value))
                callbacks[message.reply_to.Value](data);
            else if (routes.ContainsKey(message.type) && routes[message.type].ContainsKey(message.subtype ?? "null"))
                routes[message.type][message.subtype ?? "null"].DynamicInvoke(data);
            else
                System.Diagnostics.Debug.Write(string.Format("No valid route for {0} - {1}", message.type, message.subtype ?? "null"));
        }

        public void Send<K>(SlackSocketMessage message, Action<K> callback)
            where K : SlackSocketMessage
        {
            int sendingId = Interlocked.Increment(ref currentId);
            message.id = sendingId;
            callbacks.Add(sendingId, (c) =>
            {
                K obj = JsonConvert.DeserializeObject<K>(c, new JavascriptDateTimeConverter());
                callback(obj);
            });
            Send(message);
        }

        public void Send(SlackSocketMessage message)
        {
            if (message.id == 0)
                message.id = Interlocked.Increment(ref currentId);
            //socket.Send(JsonConvert.SerializeObject(message));

            SlackSocketRouting route = message.GetType().GetCustomAttribute<SlackSocketRouting>();
            if (route == null && message.type == null) throw new InvalidProgramException("Cannot send without a proper route!");

            if (route != null)
            {
                message.type = route.Type;
                message.subtype = route.SubType;
            }

            sendingQueue.Push(JsonConvert.SerializeObject(message));
            if (Interlocked.CompareExchange(ref currentlySending, 1, 0) == 0)
                ThreadPool.QueueUserWorkItem(HandleSending);
        }

        public void BindCallback<K>(Action<K> callback)
        {
            Type t = typeof(K);

            foreach (SlackSocketRouting route in t.GetCustomAttributes<SlackSocketRouting>())
            {
                if (!routes.ContainsKey(route.Type))
                    routes.Add(route.Type, new Dictionary<string, Delegate>());
                if (!routes[route.Type].ContainsKey(route.SubType ?? "null"))
                    routes[route.Type].Add(route.SubType ?? "null", callback);
                else
                    routes[route.Type][route.SubType] = Delegate.Combine(routes[route.Type][route.SubType ?? "null"], callback);
            }
        }

        public void UnbindCallback<K>(Action<K> callback)
        {
            Type t = typeof(K);
            foreach (SlackSocketRouting route in t.GetCustomAttributes<SlackSocketRouting>())
            {
                Delegate d = routes.ContainsKey(route.Type) ? (routes.ContainsKey(route.SubType ?? "null") ? routes[route.Type][route.SubType ?? "null"] : null) : null;
                if (d != null)
                {
                    Delegate newd = Delegate.Remove(d, callback);
                    routes[route.Type][route.SubType ?? "null"] = newd;
                }
            }
        }

        void HandleSending(object stateful)
        {
            string message;
            while (sendingQueue.Pop(out message))
                socket.Send(message);

            currentlySending = 0;
        }
    }

    public class SlackSocketMessage
    {
        public int id;
        public int? reply_to;
        public string type;
        public string subtype;
        public bool ok;
    }

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
