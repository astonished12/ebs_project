using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                Filter = new Filter {MaxX = 1, MaxY = 1}
            });

            Subscriber s2 = new Subscriber("Subscriber2");
            s1.ConnectToBroker("Broker1", new Message
            {
                Msg = "Connect",
                Name = "Subscriber2",
                Filter = new Filter { MaxX = 50, MaxY = 50 }
            });

        }
    }
}
