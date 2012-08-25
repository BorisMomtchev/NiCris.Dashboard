using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web.Security;
using Microsoft.ServiceModel.Web;
using NiCris.WCF.REST.Interceptors;

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

    public class WebServiceHostFactory2BasicAuth : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            WebServiceHost2 host = new WebServiceHost2(serviceType, true, baseAddresses);
            host.Interceptors.Add(new BasicAuthInterceptor(Membership.Provider, "NiCris"));
            return host;
        }
    }
}