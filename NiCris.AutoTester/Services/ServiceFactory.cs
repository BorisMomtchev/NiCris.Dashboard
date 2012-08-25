using System;
using NiCris.AutoTester.Configuration;

namespace NiCris.AutoTester.Services
{
    public static class ServiceFactory
    {
        public static IService Create(TestConfiguration config)
        {
            IService service = null;

            switch (config.ServiceType)
            {              
                //case ServiceType.SOAP:
                //    service = new SOAPService(config);
                //    break;

                //case ServiceType.SOCKET:
                //    service = new SocketService(config);
                //    break;

                default:
                    service = new RESTService(config);
                    break;
            }

            return service;
        }
    }
}
