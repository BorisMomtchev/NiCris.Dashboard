using System;
using NiCris.AutoTester.Messages;

namespace NiCris.AutoTester.Services
{
    public interface IService
    {
        string ServiceURI {get; set;}
        //Int16 ServicePort { get; set; }
        
        bool AuthRequired { get; set; }
        AuthSchema AuthType { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        string AuthToken { get; set; }

        bool IsConfigured { get; set; }
        //bool IsOnline {get;}
        
        string Send(NetMessage message);
    }

    public enum ServiceType
    {
        REST,
        //SOAP,
        //SOCKET
    }

    public enum AuthSchema
    {
        Basic,
        Token,
    }
}
