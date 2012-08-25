using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NiCris.AutoTester.Messages
{
    public interface INetMessage
    {
        string Id { get; set; }
        string Headers { get; set; }
        string Request { get; set; }
        string Responce { get; set; }

        int Index { get; set; }
        bool Processed { get; set; }
        bool SuccessfullyCompleted { get; set; }

        DateTime DateTimeSent { get; set; }
    }
}
