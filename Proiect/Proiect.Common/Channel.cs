using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using log4net.Core;
using Proiect.Common.Messages;
using ServiceStack;
using ServiceStack.Redis;

namespace Proiect.Common
{
    public class Channel
    {
        public string Name { get; set; }
        public RedisClient Client { get; set; }
        public Thread ChannelThread = null;

        public Dictionary<string, Filter> SubscribersList;
        private readonly PubSubActors Type;
        private static readonly ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Channel(PubSubActors type)
        {
            Client = new RedisClient("localhost: 6379");
            Type = type;

            SubscribersList = new Dictionary<string, Filter>();
        }

        public void StartListen(string name)
        {
            Name = name;
            ChannelThread = new Thread(delegate ()
            {
                IRedisSubscription subscription = null;
                using (subscription = Client.CreateSubscription())
                {
                    subscription.OnSubscribe = channel =>
                    {
                        Log.DebugFormat("Client {0} Subscribed to '{1}'", Name, channel);
                    };
                    subscription.OnUnSubscribe = channel =>
                    {
                        Log.DebugFormat("Client {0} UnSubscribed from {1} ", Name, channel);
                    };
                    subscription.OnMessage = (channel, msg) =>
                    {
                        Log.DebugFormat("Actor {0} received '{1}' from channel '{2}'",Name, msg, channel);
                        if (Type == PubSubActors.Broker)
                        {
                            var message = msg.FromJson<Message>();
                            if (message != null)
                            {
                                if (message.Msg.Equals("Connect"))
                                {
                                    SubscribersList.TryGetValue(message.Name, out var filter);
                                    if (filter == null)
                                    {
                                        SubscribersList.Add(message.Name, message.Filter);
                                    }
                                }
                                else if (message.Msg.Equals("Disconnect"))
                                {
                                    SubscribersList.TryGetValue(message.Name, out var filter);
                                    if (filter != null)
                                    {
                                        SubscribersList.Remove(message.Name);
                                    }
                                }
                                else if (message.Msg.Equals("Publish"))
                                {
                                    RedirectToSubscribers(message);
                                }
                            }
                        }
                    };
                }

                subscription.SubscribeToChannels(Name);
            });
            ChannelThread.Start();
        }

        private void RedirectToSubscribers(Message msg)
        {
            var payload = msg.Payload;
            foreach (var client in SubscribersList)
            {
                if (client.Value.MaxX >= payload.X && client.Value.MinX <= payload.X &&
                    client.Value.MaxY >= payload.Y && client.Value.MinY <= payload.Y)
                {
                    using (var cl = new RedisClient("localhost: 6379"))
                    {
                        cl.PublishMessage($"Subscriber_{client.Key}", message: msg.ToJson());
                    }
                }
            }
            
        }
    }
}
