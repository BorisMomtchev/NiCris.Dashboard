using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using NiCris.AutoTester.Configuration;
using NiCris.AutoTester.Services;
using NiCris.AutoTester.Logging;
using NiCris.AutoTester.Dynamic;

namespace NiCris.AutoTester.Messages
{
    public sealed class MessageFactory
    {
        // FIELDS
        private static MessageFactory _instance = null;

        private static TestConfiguration _testConfig;
        private static Dictionary<string, string> _msgParams;
        private static Dictionary<string, string> _paramMethod;
        private static string _msgTemplate = string.Empty;
        
        private static IService _service;
        private static readonly object _locker = new object();

        private static ILogger _logger;

        // PRIVATE C~tor
        MessageFactory() {}

        // SINGLETON
        public static MessageFactory Instance
        {
            get
            {
                lock (_locker)
                {
                    if (_instance == null)
                        _instance = new MessageFactory();
                    return _instance;
                }
            }
        }

        
        // PROPERTIES
        public static string XMLConfigFile { get; private set; }
        public static bool IsConfigured { get; private set; }

        public static List<NetMessage> MessageList { get; private set; }
        public static int NumOfScheduledMessages { get; private set; }
        public static string ServiceURI { get; private set; }
        public static bool DynamicLibrary { get; private set; }

        
        // MAIN METHODS
        public static void Configure(string xmlConfigFile)
        {
            _testConfig = new TestConfiguration(xmlConfigFile);
            
            MessageList = new List<NetMessage>();
            XMLConfigFile = xmlConfigFile;
            ServiceURI = _testConfig.ServiceURI;
            NumOfScheduledMessages = _testConfig.NumberOfMessages;
            DynamicLibrary = _testConfig.DynamicLibrary;
            IsConfigured = true;

            _service = ServiceFactory.Create(_testConfig);
            _msgParams = new Dictionary<string,string>();
            _paramMethod = new Dictionary<string, string>();

            // read all dynamic msg params
            foreach (XmlElement item in _testConfig.Message.SelectSingleNode("MessageParams"))
            {
                _msgParams.Add(item.GetAttribute("code"), item.GetAttribute("initialValue"));
                _paramMethod.Add(item.GetAttribute("code"), item.GetAttribute("method"));
            }

            // get the raw message/template
            XmlNode msgTemplateNode = _testConfig.Message.SelectSingleNode("MessageTemplate");
            _msgTemplate = msgTemplateNode.FirstChild.Value;
        }

        public static void CompileDynamicLibrary()
        {
            DynamicHelper.CompileAssembly(_testConfig.DynamicLibraryCode);
        }

        public string SendMessage(int index, bool detailMode)
        {
            if (!IsConfigured) throw new Exception("MessageFactory is not configured.");

            // Create messages one by one, store & finally send them to the server...
            NetMessage msg = CreateMessage(index);
            MessageList.Add(msg);
            return _service.Send(msg);
        }

        private NetMessage CreateMessage(int index)
        {
            NetMessage newMessage = new NetMessage();
            string msgBody = _msgTemplate;  
            string msgID = Guid.NewGuid().ToString();

            // Param & help variables
            string pCode; string pValue; string pMethod;
            int tInt; decimal tDecimal; DateTime tDateTime; string tString;

            foreach (string item in _msgParams.Keys)
            {
                // Get the param specifics
                pCode = item;
                pValue = _msgParams[pCode];
                pMethod = _paramMethod[pCode];

                // Initialize our temp variables
                tInt = 0;
                tDecimal = 0.0M;
                tDateTime = DateTime.Now;
                tString = string.Empty;

                switch (pCode)
                {
                    case "%ID%":            
                        msgBody = msgBody.Replace("%ID%", msgID);
                        break;

                    case "%DATETIME%":  
                        tDateTime = GetDateTimeParam(tDateTime, pMethod, pValue, index);
                        // We need to supply it in format: "2010-11-04T15:26:53.98"
                        msgBody = msgBody.Replace("%DATETIME%", string.Format("{0:s}", tDateTime));
                        break;

                    case "%NAME_ID%":    
                        tInt = GetIntParam(tInt, pMethod, pValue, index);
                        msgBody = msgBody.Replace("%NAME_ID%", tInt.ToString());
                        break;

                    case "%USER_ID%":      
                        tInt = GetIntParam(tInt, pMethod, pValue, index);
                        msgBody = msgBody.Replace("%USER_ID%", tInt.ToString());
                        break;
                 }
            }

            newMessage.Index = index;
            newMessage.Id = msgID;
            newMessage.Request = msgBody;
            
            return newMessage;
        }


        // HELP METHODS
        private string GetStringParam(string tString, string pMethod, string pValue, int index)
        {
            // Pass an initial value, call any dynamic code or use pre-set value and return...
            if (UseDynamicLibrary(pMethod))
                tString = (string)DynamicHelper.ExecuteMethod(pMethod, pValue, index);
            else if (!string.IsNullOrEmpty(pValue))
                tString = pValue;

            return tString;
        }

        private int GetIntParam(int tInt, string pMethod, string pValue, int index)
        {
            if (UseDynamicLibrary(pMethod))
                tInt = (int)DynamicHelper.ExecuteMethod(pMethod, pValue, index);
            else if (!string.IsNullOrEmpty(pValue))
                tInt = index + int.Parse(pValue);   // increment all ints by default with index

            return tInt;
        }

        private decimal GetDecimalParam(decimal tDecimal, string pMethod, string pValue, int index)
        {
            if (UseDynamicLibrary(pMethod))
                tDecimal = (decimal)DynamicHelper.ExecuteMethod(pMethod, pValue, index);
            else if (!string.IsNullOrEmpty(pValue))
                tDecimal = decimal.Parse(pValue);

            return tDecimal;
        }

        private DateTime GetDateTimeParam(DateTime tDateTime, string pMethod, string pValue, int index)
        {
            if (UseDynamicLibrary(pMethod))
                tDateTime = (DateTime)DynamicHelper.ExecuteMethod(pMethod, pValue, index);
            else if (!string.IsNullOrEmpty(pValue))
                tDateTime = DateTime.Parse(pValue);

            return tDateTime;
        }

        private bool UseDynamicLibrary(string paramMethod)
        {
            if (DynamicLibrary && !string.IsNullOrEmpty(paramMethod))
                return true;
            return false;
        }

        public static int NumOfSentMessages()
        {
            if (!IsConfigured) throw new Exception("MessageFactory is not configured.");

            int counter = 0;

            foreach (NetMessage msg in MessageList)
                if (msg.Processed)
                    counter++;

            return counter;
        }

        public static int GetAllFailed()
        {
            if (!IsConfigured) throw new Exception("MessageFactory is not configured.");

            int counter = 0;

            foreach (NetMessage msg in MessageList)
                if (!msg.SuccessfullyCompleted) 
                    counter++;

            return counter;
        }

        public NetMessage GetMessageByIndex(int index)
        {
            if (!IsConfigured) throw new Exception("MessageFactory is not configured.");

            if (MessageList.Count >= index && index > 0)
                return MessageList[index - 1];
            return null;
        }

        public static string CreateReport()
        {
            if (!IsConfigured) throw new Exception("MessageFactory is not configured.");
            if (MessageList.Count == 0) return string.Empty;

            StringBuilder summary = new StringBuilder();
            
            summary.Append("\n-------\n");
            summary.Append("SUMMARY ");
            summary.Append(DateTime.Now.ToString());
            summary.Append("\nConfig File Used: ");
            summary.Append(XMLConfigFile.ToLower());
            summary.Append("\n-------\n");

            summary.Append("Scheduled messages: ");
            summary.Append(NumOfScheduledMessages);

            summary.Append("\nGenerated messages: ");
            summary.Append(MessageList.Count);

            summary.Append("\nSent messages: ");
            summary.Append(NumOfSentMessages());

            summary.Append("\nFailed messages: ");
            summary.Append(GetAllFailed());

            summary.Append("\nDONE.\n");

            // Write all reports to disk - Summary log
            _logger = LoggerFactory.Create(string.Empty, XMLConfigFile + "_Summary");
            _logger.Info(summary.ToString().Replace("\n", "\r\n"));

            // Detailed log
            DetailedMessageLog();

            // Return the SummaryReport to the Console
            return summary.ToString();
        }

        private static void DetailedMessageLog()
        {
            _logger = LoggerFactory.Create(string.Empty, XMLConfigFile + "_Detailed");
            
            StringBuilder msgLog = new StringBuilder();
            msgLog.Append("\nConfig File Used: ");
            msgLog.Append(XMLConfigFile.ToLower());
            msgLog.Append("\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n");

            foreach (NetMessage msg in MessageList)
            {
                msgLog.Append("Message ID: "); msgLog.Append(msg.Id);
                msgLog.Append("\nTime Sent: "); msgLog.Append(msg.DateTimeSent);
                msgLog.Append("\nSuccessfully Completed: "); msgLog.Append(msg.SuccessfullyCompleted); 
                msgLog.Append("\n");

                if (_testConfig.LogRequest)
                {
                    msgLog.Append("\nFull Request:\n"); msgLog.Append(msg.Request);
                }
                if (_testConfig.LogResponce)
                {
                    msgLog.Append("\nFull Responce:\n"); msgLog.Append(msg.Responce);
                }
                
                msgLog.Append("\n-------\n");
            }

            _logger.Info(msgLog.ToString().Replace("\n", "\r\n"));
        }
    }
}
