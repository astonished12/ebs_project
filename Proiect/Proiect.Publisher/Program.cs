using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proiect.Common;
using Proiect.Common.Messages;

namespace Proiect.Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            Publisher p1 = new Publisher("Publisher1", "Broker1");

            for (int i = 0; i < 5; i++)
            {
                p1.PublishMessage(new Message{
                    Msg = "Publish",
                    Filter = null,
                    Name = String.Empty,
                    Payload = new Payload { X = 2, Y = 5 }});
            }
        }
    }
}
