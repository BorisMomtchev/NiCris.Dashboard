using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace NiCris.WCF.REST.Utility
{
    /// <summary>
    /// Helper class to map request parameters to usable form
    /// </summary>
    public class ServiceParameterMapper : IServiceParameterMapper
    {
        /// <summary>
        /// Parces query string of the form key1=value1&key2=value2 to a dynamic map
        /// </summary>
        /// <param name="queryStering">the query string to parse</param>
        /// <returns>a dynamic collection to use the parsed parameters</returns>
        public virtual dynamic ParseQueryString(string queryStering)
        {
            var dict = new Dictionary<string, string>();
            var pairs = queryStering.Split('&');
            foreach (var pair in pairs)
            {
                var keyval = pair.Split('=');
                if (keyval.Length == 2)
                {
                    var key = keyval[0];
                    var value = HttpUtility.UrlDecode(keyval[1]);
                    if (dict.ContainsKey(key))
                    {
                        dict[key] += String.Format(",{0}", value);
                    }
                    else
                    {
                        dict.Add(key, value);
                    }
                }
            }

            return new DynamicParameterData(dict);
        }

        /// <summary>
        /// Parces the request body of the form key1=value1&key2=value2 to a dynamic map
        /// </summary>
        /// <param name="requestStream">the request body stream</param>
        /// <returns>a dynamic collection to use the parsed parameters</returns>
        public virtual dynamic ParseRequestBody(Stream requestStream)
        {
            var data = new StreamReader(requestStream).ReadToEnd();
            return ParseQueryString(data);
        }
    }
}