using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;

namespace NiCris.WCF.REST.Utility
{
    public class URLParamtersHelper
    {
        public static string[] ParseRepeatedParameters(Stream input, params string[] list)
        {

            string[] inputParams = new StreamReader(input).ReadToEnd().Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            //post body like: test=d+d+d&test=d%2bd+d&test=d%25d%26d

            StringBuilder[] parameters = new StringBuilder[list.Length];
            for(int i= 0; i < list.Length; ++i)
                parameters[i] = new StringBuilder();

            foreach (var param in inputParams)
            {
                if (string.IsNullOrEmpty(param))
                    continue;

                for (int i = 0; i < list.Length; ++i)
                {
                    if (param.StartsWith(list[i], StringComparison.Ordinal))
                    {
                        parameters[i].AppendFormat("{0},", HttpUtility.UrlDecode(param.Substring(list[i].Length + 1)));
                    }
                }
            }

            return parameters.Select(x => x.ToString(0, x.Length > 0 ? x.Length -1 : x.Length)).ToArray();
        }

        public static DateTime GetDate(string date)
        {
            if (string.IsNullOrEmpty(date))
                throw new ArgumentNullException("from | to");

            DateTime dt;
            if (!DateTime.TryParse(date, out dt))
            {
                throw new FormatException("The supplied date parameter cannot be converted to a .NET DateTime type");
            }

            return dt;
        }

        public static DateTime? GetDateOrNull(string date)
        {
            if (string.IsNullOrEmpty(date))
                return null;

            DateTime dt;
            if (!DateTime.TryParse(date, out dt))
            {
                throw new FormatException("The supplied date parameter cannot be converted to a .NET DateTime type");
            }

            return dt;
        }


    }
}