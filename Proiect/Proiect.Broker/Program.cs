using System;
using System.Configuration;
using System.Threading;

namespace Proiect.Broker
{
    class Program
    {
        static void Main(string[] args)
        {
            Broker b1 = new Broker("Broker1");
            Broker b2 = new Broker("Broker2");

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
