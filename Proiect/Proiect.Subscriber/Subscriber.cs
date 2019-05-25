using Proiect.Common;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace Proiect.Subscriber
{
    public class Subscriber
    {
        private readonly Channel Channel;
        private ISerializer Serializer;
        public Subscriber(string name)
        {
            Channel = new Channel(PubSubActors.Subscriber);
            Channel.StartListen($"Subscriber_{name}");
            Serializer = new NewtonsoftSerializer();

        }

        public void ConnectToBroker(string brokerName, object message)
        {
            var publisher = Channel.Client.GetSubscriber();
            publisher.PublishAsync($"Broker_{brokerName}", Serializer.Serialize(message));
        }

        public void DisconnectFromBroker(string brokerName)
        {
            var publisher = Channel.Client.GetSubscriber();
            publisher.PublishAsync($"Broker_{brokerName}", "disconnect");
        }
    }
}
