using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proiect.Common;
using ServiceStack;

namespace Proiect.Publisher
{
    public class Publisher
    {
        private readonly Channel Channel;
        public string BrokerName { get; set; }

        public Publisher(string name, string brokerName)
        {
            Channel = new Channel(PubSubActors.Publisher);
            BrokerName = brokerName;
        }

        public void PublishMessage(object message)
        {
            Channel.Client.PublishMessage($"Broker_{BrokerName}", message.ToJson());
        }
    }
}
