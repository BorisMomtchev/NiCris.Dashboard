using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Activation;
using System.ServiceModel;
using Microsoft.ServiceModel.Web;

namespace NiCris.WCF.REST
{
    public class WebServiceHostFactory2 : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            WebServiceHost2 host = new WebServiceHost2(serviceType, false, baseAddresses);
            return host;
        }
    }
}