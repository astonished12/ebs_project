﻿using System;
using Proiect.Common;
using Proiect.Common.Messages;

namespace Proiect.Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            Subscriber s1 = new Subscriber("Subscriber1");
            s1.ConnectToBroker("Broker1", new Message
            {
                Msg = "Connect",
                Name = "Subscriber1",
                Filter = new Filter {MaxX = 100, MinX = 0, MinY = 0, MaxY = 100}
            });

            Subscriber s2 = new Subscriber("Subscriber2");
            s1.ConnectToBroker("Broker1", new Message
            {
                Msg = "Connect",
                Name = "Subscriber2",
                Filter = new Filter { MaxX = 100, MinX = 1, MaxY = 100 , MinY = 1}
            });

            Subscriber s3 = new Subscriber("Subscriber3");
            s1.ConnectToBroker("Broker2", new Message
            {
                Msg = "Connect",
                Name = "Subscriber3",
                Filter = new Filter { MaxX = 100, MinX = 0, MaxY = 100, MinY = 0}
            });

            Console.ReadKey();


        }
    }
}
