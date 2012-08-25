using System;
using NiCris.AutoTester.Services;
using System.Xml;

namespace NiCris.AutoTester.Configuration
{
    public class TestConfiguration
    {
        public string XmlConfigFile { get; private set; }

        public string ServiceURI { get; private set; }
        public ServiceType ServiceType { get; private set; }
        public string ExecuteVerbs { get; private set; }

        public bool AuthRequired { get; private set; }
        public AuthSchema AuthType { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string AuthToken { get; private set; }
        
        public int NumberOfMessages { get; private set; }
        public int MessageDelay { get; private set; }
        public bool DynamicLibrary { get; private set; }
        public string[] DynamicLibraryCode { get; private set; }

        public bool LogRequest { get; private set; }
        public bool LogResponce { get; private set; }

        public XmlElement Message { get; private set; }

        public TestConfiguration(string xmlConfigFile)
        {
            XmlConfigFile = xmlConfigFile;

            XmlDocument xmlConfig = new XmlDocument();
            xmlConfig.Load(XmlConfigFile);

            // Get the basic configuration for all types of messages
            XmlElement configGroup = (XmlElement)xmlConfig.DocumentElement.SelectSingleNode("Configuration");

            this.ServiceURI = configGroup.GetAttribute("ServiceURI");

            string serviceType = configGroup.GetAttribute("ServiceType");
            if (!string.IsNullOrEmpty(serviceType))
                this.ServiceType = (ServiceType)Enum.Parse(typeof(ServiceType), serviceType);
            
            this.ExecuteVerbs = configGroup.GetAttribute("ExecuteVerbs");
            this.AuthRequired = bool.Parse(configGroup.GetAttribute("AuthRequired"));
            
            string authSchema = configGroup.GetAttribute("AuthSchema");
            if (!string.IsNullOrEmpty(authSchema))
                this.AuthType = (AuthSchema)Enum.Parse(typeof(AuthSchema), authSchema);

            this.Username = configGroup.GetAttribute("Username");
            this.Password = configGroup.GetAttribute("Password");
            this.AuthToken = configGroup.GetAttribute("AuthToken");

            this.NumberOfMessages = int.Parse(configGroup.GetAttribute("NumberOfMessages"));
            this.MessageDelay = int.Parse(configGroup.GetAttribute("MessageDelay"));
            
            string dynamicLibrary = configGroup.GetAttribute("DynamicLibrary");
            if (!string.IsNullOrEmpty(dynamicLibrary))
                this.DynamicLibrary = bool.Parse(dynamicLibrary);

            this.LogRequest = bool.Parse(configGroup.GetAttribute("LogRequest"));
            this.LogResponce = bool.Parse(configGroup.GetAttribute("LogResponce"));


            // The whole message structure as defined in the xml config file...
            this.Message = (XmlElement)xmlConfig.DocumentElement.SelectSingleNode("Message");


            // Dynamic code...
            if (this.DynamicLibrary)
            {
                string dynamicLibrarySource =
                    xmlConfig.DocumentElement.SelectSingleNode("DynamicLibrarySource")
                    .FirstChild.Value;

                this.DynamicLibraryCode = new string[] { dynamicLibrarySource };
            }
        }
    }
}
