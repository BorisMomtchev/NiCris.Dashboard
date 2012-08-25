using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NiCris.AutoTester.Messages
{
    public class NetMessage : INetMessage
    {
        public string Id { get; set; }
        public string Headers { get; set; }
        public string Request { get; set; }
        public string Responce { get; set; }

        public int Index { get; set; }
        public bool Processed { get; set; }
        public bool SuccessfullyCompleted { get; set; }

        public DateTime DateTimeSent { get; set; }
    }
}
