using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Proiect.Common;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace Proiect.Publisher
{
    public class Publisher
    {
        private readonly Channel Channel;
        public string BrokerName { get; set; }
        private ISerializer Serializer;

        public Publisher(string name, string brokerName)
        {
            Channel = new Channel(PubSubActors.Publisher);
            BrokerName = brokerName;
            Serializer = new NewtonsoftSerializer();
        }

        public void PublishMessage(object message)
        {
            Channel.Client.GetSubscriber().PublishAsync($"Broker_{BrokerName}", JsonConvert.SerializeObject(message));
        }
    }
}
