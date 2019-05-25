using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proiect.Common.Messages
{
    public class Message
    {
        public string Msg { get; set; }
        public string Name { get; set; }
        public Payload Payload { get; set; }
        public Filter Filter { get; set; }
    }
}
