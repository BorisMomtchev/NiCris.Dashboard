using System.Net;
using System.Text;
using NiCris.AutoTester.Configuration;
using NiCris.AutoTester.Messages;
using Microsoft.Http;
using Microsoft.Http.Headers;
using System;

namespace NiCris.AutoTester.Services
{
    public class RESTService : IService
    {
        private HttpClient _client;
        private TestConfiguration _testConfig { get; set; }

        public RESTService(TestConfiguration config)
        {
            _testConfig = config;

            ServiceURI = _testConfig.ServiceURI;
            //ServicePort = TestConfig.ServicePort;

            AuthRequired = _testConfig.AuthRequired;
            AuthType = (AuthSchema)_testConfig.AuthType;
            Username = _testConfig.Username;
            Password = _testConfig.Password;
            AuthToken = _testConfig.AuthToken;
            IsConfigured = true;
            //IsOnline = true;

            // Now that the service is configured, create an http client that can talk to it...
            _client = new HttpClient();
            
            // Is auth required?
            if (AuthRequired)
            {
                if (AuthType == AuthSchema.Basic)
                    _client.TransportSettings.Credentials = new NetworkCredential(Username, Password);
                else if (AuthType == AuthSchema.Token)
                    _client.DefaultHeaders.Authorization = new Credential(AuthToken);
            }
        }

        #region IService Members

        public string ServiceURI { get; set; }
        //public Int16 ServicePort { get; set; }

        public bool AuthRequired { get; set; }
        public AuthSchema AuthType { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AuthToken { get; set; }
        public bool IsConfigured { get; set; }

        public string Send(NetMessage message)
        {
            System.Threading.Thread.Sleep(_testConfig.MessageDelay);
            message.DateTimeSent = DateTime.Now;

            // TODO: Add the detailed string...
            StringBuilder builder = new StringBuilder();

            if (!string.IsNullOrEmpty(_testConfig.ExecuteVerbs))
            {
                if (_testConfig.ExecuteVerbs.Contains("POST"))
                {
                    builder.Append(POST(message, true)); builder.Append("|");
                }

                if (_testConfig.ExecuteVerbs.Contains("GET"))
                {
                    builder.Append(GET(message)); builder.Append("|");
                }

                if (_testConfig.ExecuteVerbs.Contains("PUT"))
                {
                    builder.Append(PUT(message)); builder.Append("|");
                }

                if (_testConfig.ExecuteVerbs.Contains("DELETE"))
                {
                    builder.Append(DELETE(message)); builder.Append("|");
                }

                return string.Format(" Series {0} Completed ", builder.ToString());
            }
            else
            {
                return POST(message, false);   // Default is POST
            }
        }

        private string POST(NetMessage message, bool inSeries)
        {
            HttpContent httpContent = HttpContent.Create(message.Request, "application/xml");
            string statusCode = string.Empty;

            using (HttpResponseMessage response = _client.Post(ServiceURI, httpContent))
            {
                message.Processed = true;
                message.SuccessfullyCompleted = (response.StatusCode == HttpStatusCode.Created);
                message.Responce = response.Content.ReadAsString();
                statusCode = response.StatusCode.ToString();

                // Set the Id for further processing; AbsolutePath is something like /BizMsgService/314
                message.Id = response.Headers.Location.AbsolutePath.Split('/')[2];
            }

            if (inSeries)
            {
                if (message.SuccessfullyCompleted) return "C";
                return statusCode;
            }
            return string.Format(" Response Code: {0} ", statusCode);
        }

        private string GET(NetMessage message)
        {
            string statusCode = string.Empty;

            using (HttpResponseMessage response = _client.Get(ServiceURI + message.Id))
            {
                message.Processed = true;
                message.SuccessfullyCompleted = (response.StatusCode == HttpStatusCode.OK);
                message.Responce = response.Content.ReadAsString();
                statusCode = response.StatusCode.ToString();
            }

            if (message.SuccessfullyCompleted) return "R";
            return statusCode;
        }

        private string PUT(NetMessage message)
        {
            HttpContent httpContent = HttpContent.Create(message.Request, "application/xml");
            string statusCode = string.Empty;

            using (HttpResponseMessage response = _client.Put(ServiceURI + message.Id, httpContent))
            {
                message.Processed = true;
                message.SuccessfullyCompleted = (response.StatusCode == HttpStatusCode.Accepted);
                message.Responce = response.Content.ReadAsString();
                statusCode = response.StatusCode.ToString();
            }
            
            if (message.SuccessfullyCompleted) return "U";
            return statusCode;
        }

        private string DELETE(NetMessage message)
        {
            string statusCode = string.Empty;

            using (HttpResponseMessage response = _client.Delete(ServiceURI + message.Id))
            {
                message.Processed = true;
                message.SuccessfullyCompleted = (response.StatusCode == HttpStatusCode.OK);
                message.Responce = response.Content.ReadAsString();
                statusCode = response.StatusCode.ToString();
            }

            if (message.SuccessfullyCompleted) return "D";
            return statusCode;
        }

        #endregion
    }
}
