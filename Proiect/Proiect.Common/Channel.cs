using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using log4net.Core;
using Newtonsoft.Json;
using Proiect.Common.Messages;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace Proiect.Common
{
    public class Channel
    {
        public string Name { get; set; }
        public ConnectionMultiplexer Client { get; set; }
        public Thread ChannelThread = null;

        public Dictionary<string, Filter> SubscribersList;
        private readonly PubSubActors Type;
        private static readonly ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ISerializer Serializer;

        public Channel(PubSubActors type)
        {
            Client = ConnectionMultiplexer.Connect("localhost: 6379");
            Type = type;

            SubscribersList = new Dictionary<string, Filter>();
            Serializer = new NewtonsoftSerializer();
        }

        public void StartListen(string name)
        {
            Name = name;
            ChannelThread = new Thread(delegate ()
            {
                ISubscriber subscription = Client.GetSubscriber();
                subscription.Subscribe(Name, (channel, msg) =>
                {
                    Log.DebugFormat("Actor {0} received '{1}' from channel '{2}'", Name, msg, channel);
                    if (Type == PubSubActors.Broker)
                    {
                        var message = JsonConvert.DeserializeObject<Message>(msg);
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

                });
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
                    var publisher = Client.GetSubscriber();
                    publisher.PublishAsync($"Subscriber_{client.Key}", JsonConvert.SerializeObject(msg));
                }
            }
        }

    }
}

