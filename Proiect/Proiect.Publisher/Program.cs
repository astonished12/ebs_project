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
            Publisher p2 = new Publisher("Publisher2", "Broker2");
            var rnd = new Random();

            Stopwatch sw = Stopwatch.StartNew();
            Stopwatch sw1 = Stopwatch.StartNew();

            int totalMessages = 0;
            while (sw.ElapsedMilliseconds < 60000)
            {
                if (sw1.ElapsedMilliseconds > 10)
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

                    totalMessages += 2;
                    sw1.Restart();
                }
            }

            sw.Stop();
            sw1.Stop();

            p1.PublishMessage(new Message()
            {
                Msg = "Stop"
            });

            Console.WriteLine($"Total messages sent {totalMessages}");
        }
    }
}
