using Proiect.Common;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace Proiect.Subscriber
{
    public class Subscriber
    {
        private readonly Channel _channel;
        private readonly ISerializer _serializer;
        public string Name { get; set; }
        public Filter Filter { get; set; }
        public bool IsConnected { get; set; }

        public Subscriber(string name, Filter filter)
        {
            Name = name;
            Filter = filter;

            _channel = new Channel(PubSubActors.Subscriber);
            _channel.StartListen($"Subscriber_{name}");
            _serializer = new NewtonsoftSerializer();

        }


        public void ConnectToBroker(string brokerName, object message)
        {
            var publisher = _channel.Client.GetSubscriber();
            publisher.PublishAsync($"Broker_{brokerName}", _serializer.Serialize(message));
        }

        public void DisconnectFromBroker(string brokerName, object message)
        {
            var publisher = _channel.Client.GetSubscriber();
            publisher.PublishAsync($"Broker_{brokerName}", _serializer.Serialize(message));
        }
    }
}
