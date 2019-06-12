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

        public static Dictionary<string, Filter> SubscribersList = new Dictionary<string, Filter>();
        private readonly PubSubActors Type;
        private static readonly ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string IpConnection = "52.143.161.188: 6379";

        private int _totalMsgReceived = 0;

        public Channel(PubSubActors type)
        {
            Client = ConnectionMultiplexer.Connect(IpConnection);
            Type = type;
        }

        public void StartListen(string name)
        {
            Name = name;
            ChannelThread = new Thread(delegate ()
            {
                ISubscriber subscription = Client.GetSubscriber();
                subscription.Subscribe(Name, (channel, msg) =>
                {
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
                                    Log.DebugFormat("{0} connected to {1}", message.Name, name);
                                }
                            }
                            else if (message.Msg.Equals("Disconnect"))
                            {
                                if (SubscribersList.ContainsKey(message.Name))
                                {
                                    SubscribersList.Remove(message.Name);
                                    Log.DebugFormat("{0} disconnected to {1}", message.Name, name);
                                }
                            }
                            else if (message.Msg.Equals("Publish"))
                            {
                                RedirectToSubscribers(message);
                            }
                            else if (message.Msg.Equals("Stop"))
                            {
                                foreach (var client in SubscribersList)
                                {
                                    var publisher = Client.GetSubscriber();
                                    publisher.PublishAsync($"Subscriber_{client.Key}", "Stop");
                                }
                            }
                        }
                    }
                    else if (Type == PubSubActors.Subscriber)
                    {
                        Log.DebugFormat("Subscriber {0} received '{1}' from channel '{2}'", Name, msg, channel);
                        _totalMsgReceived += 1;
                        if (msg.Equals("Stop"))
                        {
                            Log.DebugFormat("Total msg received is {0}", _totalMsgReceived);
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

