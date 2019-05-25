using Proiect.Common;

namespace Proiect.Broker
{
    public class Broker
    {
        private readonly Channel Channel;
        public Broker(string name)
        {
            Channel = new Channel(PubSubActors.Broker);
            Channel.StartListen($"Broker_{name}");
        }
    }
}
