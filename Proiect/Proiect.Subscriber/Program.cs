using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Proiect.Common;
using Proiect.Common.Messages;

namespace Proiect.Subscriber
{
    class Program
    {
        private static readonly List<Subscriber> subscribers = new List<Subscriber>();
        private static readonly List<string> _brokerNames = new List<string>();

        static void Main(string[] args)
        {
            var s1 = new Subscriber("Subscriber1", new Filter { MaxX = 100, MinX = 0, MaxY = 100, MinY = 0 });
            var s2 = new Subscriber("Subscriber2", new Filter { MaxX = 100, MinX = 0, MaxY = 100, MinY = 0 });
            var s3 = new Subscriber("Subscriber3", new Filter { MaxX = 100, MinX = 0, MaxY = 100, MinY = 0 });

            subscribers.Add(s1);
            subscribers.Add(s2);
            subscribers.Add(s3);

            _brokerNames.Add("Broker1");
            _brokerNames.Add("Broker2");

            var r = new Random();
            foreach (var subscriber in subscribers)
            {
                HandleBrokerConnection(_brokerNames.OrderBy(x => Guid.NewGuid()).FirstOrDefault(), subscriber, true);
            }

            var aNewThread = new Thread(
                () =>
                {
                    while (true)
                    {
                        Thread.Sleep(30000);
                        foreach (var subscriber in subscribers)
                        {
                            HandleBrokerConnection(_brokerNames.OrderBy(x => Guid.NewGuid()).FirstOrDefault(), subscriber, r.Next(100) < 50);
                        }
                    }
                }
            );
          
            aNewThread.Start();
            Thread.Sleep(Timeout.Infinite);
        }

        private static void HandleBrokerConnection(string brokerName, Subscriber subscriber, bool connect)
        {
            if (connect)
            {
                if(subscriber.IsConnected) return;
                subscriber.ConnectToBroker(brokerName, new Message
                {
                    Msg = "Connect",
                    Name = subscriber.Name,
                    Filter = subscriber.Filter
                });

                Console.WriteLine($"{subscriber.Name} connected to {brokerName}");
                subscriber.IsConnected = true;
            }
            else
            {
                if(!subscriber.IsConnected) return;
                subscriber.DisconnectFromBroker(brokerName, new Message
                {
                    Msg =  "Disconnect",
                    Name = subscriber.Name
                });
                Console.WriteLine($"{subscriber.Name} disconnected from {brokerName}");
                subscriber.IsConnected = false;
            }
        }


    }
}
