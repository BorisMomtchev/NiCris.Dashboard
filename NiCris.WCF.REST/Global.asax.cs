using System;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;

namespace NiCris.WCF.REST
{
    public class Global : HttpApplication
    {
        private void RegisterRoutes()
        {
            // Non Security
            RouteTable.Routes.Add(new ServiceRoute("BizMsgService", new WebServiceHostFactory2(), typeof(BizMsgService)));
            
            // Win Auth
            RouteTable.Routes.Add(new ServiceRoute("BizMsgServiceWinAuth", new WebServiceHostFactory2(), typeof(BizMsgServiceWinAuth)));

            // Basic Auth thru Interceptor
            RouteTable.Routes.Add(new ServiceRoute("BizMsgServiceBasicAuth", new WebServiceHostFactory2BasicAuth(), typeof(BizMsgServiceBasicAuth)));
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes();
        }
    }
}
