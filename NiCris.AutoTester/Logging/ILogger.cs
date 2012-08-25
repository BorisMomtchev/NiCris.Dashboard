using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NiCris.AutoTester.Logging
{
    /// <summary>
    /// Abstract our logging with an interface, so we can easily swap loggers if needed later.
    /// This interface is easily implemented by NLog & Log4Net.
    /// </summary>    
    public interface ILogger
    {
        void Info(string message);
        void Warn(string message);
        void Debug(string message);
        void Error(string message);
        void Error(Exception ex);
        void Fatal(string message);
        void Fatal(Exception ex);
    }
}
