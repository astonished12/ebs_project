using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proiect.Common;
using ServiceStack;
using ServiceStack.Redis;

namespace Proiect.Subscriber
{
    public class Subscriber
    {
        private readonly Channel Channel;

        public Subscriber(string name)
        {
            Channel = new Channel(PubSubActors.Subscriber);
            Channel.StartListen($"Subscriber_{name}");
        }

        public void ConnectToBroker(string brokerName, object message)
        {
            using (var client = new RedisClient("localhost: 6379"))
            {
                client.PublishMessage($"Broker_{brokerName}", message.ToJson());
            }
        }

        public void DisconnectFromBroker(string brokerName)
        {
            Channel.Client.PublishMessage($"Broker_{brokerName}", "disconnect");
        }
    }
}
