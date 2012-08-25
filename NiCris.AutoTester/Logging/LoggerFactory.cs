using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NiCris.AutoTester.Logging
{
    /// <summary>
    /// Represents a logging factory.
    /// </summary>
    public static class LoggerFactory
    {
        /// <summary>
        /// Creates an ILogger instance in the specified log path.
        /// </summary>
        /// <param name="logPath">A <see cref="String"/> representing the Log path value.</param>
        /// <returns></returns>
        public static ILogger Create(string logPath, string configFile)
        {
            ILogger logger = new BasicLogger(logPath, configFile);
            return logger;
        }

        /// <summary>
        /// Creates an ILogger instance in the specified log path, and of the specified type.
        /// </summary>
        /// <param name="logPath">A <see cref="String"/> representing the Log path value.</param>
        /// <param name="loggerType">Type of the logger.</param>
        /// <returns></returns>
        public static ILogger Create(string logPath, string configFile, LoggerType loggerType)
        {
            ILogger logger = new BasicLogger(logPath, configFile);

            switch (loggerType)
            {
                //case LoggerType.NLog:
                //    logger = new NLogLogger();
                //    break;

                default:
                    // returns the default - BasicLogger implementation
                    break;
            }

            return logger;
        }
    }

    /// <summary>
    /// Represents the available logger types.
    /// </summary>
    public enum LoggerType
    {
        /// <summary>
        /// A Basic Logger.
        /// </summary>
        BasicLogger,
        //NLog,
    }

    /// <summary>
    /// Represents the available logger message types.
    /// </summary>
    public enum LogMessageType
    {
        /// <summary>
        /// An informational message.
        /// </summary>
        Info,
        /// <summary>
        /// A warning message.
        /// </summary>
        Warn,
        /// <summary>
        /// A message for debugging.
        /// </summary>
        Debug,
        /// <summary>
        /// A non-fatal error message.
        /// </summary>
        Error,
        /// <summary>
        /// A fatal error message.
        /// </summary>
        Fatal
    }
}
