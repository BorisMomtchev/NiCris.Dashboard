using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NiCris.AutoTester.Logging
{
    internal static class LogHelper
    {
        // Returns filename in the format: YYYMMDD
        internal static string GetFileNameYYYMMDD(string suffix, string extension)
        {
            return System.DateTime.Now.ToString("yyyy-MM-dd")
                + suffix
                + extension;
        }

        internal static string BuildExceptionMessage(Exception ex, bool fatal)
        {
            string exMessage =
                string.Format("Date: {0}, [Exception]\n Fatal: {1}\n Message: {2}\n Stack: {3}",
                DateTime.Now.ToString(),
                fatal.ToString(),
                ex.Message,
                ex.StackTrace);

            // Include the Data collection
            exMessage += "\n Data:";
            foreach (var item in ex.Data.Keys)
                exMessage += string.Format(" key:{0}, value:{1};", item, ex.Data[item]);

            // Are there any inner exceptions?
            while (ex.InnerException != null)
            {
                exMessage += BuildInnerExceptionMessage(ex.InnerException);
                ex = ex.InnerException;
            }

            return exMessage;
        }

        private static string BuildInnerExceptionMessage(Exception ex)
        {
            string inExMessage =
                string.Format("\n  [Inner Exception]\n  Message: {0}\n  Stack: {1}",
                ex.Message,
                ex.StackTrace);

            // Include the Data collection
            inExMessage += "\n  Data:";
            foreach (var item in ex.Data.Keys)
                inExMessage += string.Format(" key:{0}, value:{1};", item, ex.Data[item]);

            return inExMessage;
        }
    }
}
