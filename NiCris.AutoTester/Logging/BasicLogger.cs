using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NiCris.AutoTester.Logging
{
    internal class BasicLogger : ILogger
    {
        const string _fileExt = ".log";

        internal string ConfigFile { get; set; }
        internal string LogPath { get; private set; }

        static readonly object _locker = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicLogger"/> class.
        /// </summary>
        /// <param name="logPath">A <see cref="String"/> representing the Log path value.</param>
        internal BasicLogger(string logPath, string configFile)
        {
            LogPath = logPath;
            ConfigFile = "_" + configFile.ToLower();
        }

        #region ILogger Members

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="message">A <see cref="String"/> representing the Message value.</param>
        public void Info(string message)
        {
            WriteLog(message, LogMessageType.Info);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">A <see cref="String"/> representing the Message value.</param>
        public void Warn(string message)
        {
            WriteLog(message, LogMessageType.Warn);
        }

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">A <see cref="String"/> representing the Message value.</param>
        public void Debug(string message)
        {
            WriteLog(message, LogMessageType.Debug);
        }

        /// <summary>
        /// logs an error message.
        /// </summary>
        /// <param name="message">A <see cref="String"/> representing the Message value.</param>
        public void Error(string message)
        {
            WriteLog(message, LogMessageType.Error);
        }

        /// <summary>
        /// Logs an error message from the specified exception.
        /// </summary>
        /// <param name="ex">A <see cref="Exception"/> representing the Ex value.</param>
        public void Error(Exception ex)
        {
            WriteLog(ex, false);
        }

        /// <summary>
        /// Logs a fatal error message.
        /// </summary>
        /// <param name="message">A <see cref="String"/> representing the Message value.</param>
        public void Fatal(string message)
        {
            WriteLog(message, LogMessageType.Fatal);
        }

        /// <summary>
        /// Logs a fatal error message from the specified exception.
        /// </summary>
        /// <param name="ex">A <see cref="Exception"/> representing the Ex value.</param>
        public void Fatal(Exception ex)
        {
            WriteLog(ex, true);
        }

        #endregion

        #region Private Help Methods

        public void WriteLog(String message, LogMessageType messageType)
        {
            StreamWriter sw = null;

            // Protect the logging code with a try
            try
            {
                lock (_locker)
                {
                    string filename = LogPath + LogHelper.GetFileNameYYYMMDD(ConfigFile, _fileExt);
                    sw = new StreamWriter(filename, true);  // true means Append & default encoding is UTF8

                    string textEntry = string.Format("Date: {0}, [{1}] {2}",
                        DateTime.Now.ToString(), messageType.ToString(), message);

                    sw.WriteLine(textEntry);
                }
            }
            catch (Exception) { }
            finally { if (sw != null) sw.Close(); }
        }

        public void WriteLog(Exception ex, bool fatal)
        {
            StreamWriter sw = null;

            // Protect the logging code with a try
            try
            {
                lock (_locker)
                {
                    string fileName = LogPath + LogHelper.GetFileNameYYYMMDD(ConfigFile, _fileExt);
                    sw = new StreamWriter(fileName, true);

                    string textEntry = LogHelper.BuildExceptionMessage(ex, fatal);

                    sw.WriteLine(textEntry);
                }
            }
            catch (Exception) { }
            finally { if (sw != null) sw.Close(); }
        }

        #endregion
    }
}
