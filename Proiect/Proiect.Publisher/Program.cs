using System;
using System.Diagnostics;
using Proiect.Common;
using Proiect.Common.Messages;

namespace Proiect.Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            Publisher p1 = new Publisher("Publisher1", "Broker1");
            Publisher p2 = new Publisher("Publisher1", "Broker2");
            var rnd = new Random();


            Stopwatch sw = Stopwatch.StartNew();

            while (sw.ElapsedMilliseconds < 30000)
            {
                p1.PublishMessage(new Message
                {
                    Msg = "Publish",
                    Filter = null,
                    Name = String.Empty,
                    Payload = new Payload { X = rnd.Next(1, 100), Y = rnd.Next(1, 100) }
                });

                p2.PublishMessage(new Message
                {
                    Msg = "Publish",
                    Filter = null,
                    Name = String.Empty,
                    Payload = new Payload { X = rnd.Next(1, 100), Y = rnd.Next(1, 100) }
                });
            }

            sw.Stop();
            p1.PublishMessage(new Message()
            {
                Msg = "Stop"
            });

        }
    }
}
