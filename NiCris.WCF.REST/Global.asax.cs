using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Web;
using System.Web.Routing;

namespace NiCris.WCF.REST
{
    public class Global : HttpApplication
    {
        private void RegisterRoutes()
        {
            RouteTable.Routes.Add(new ServiceRoute("BizMsgService", new WebServiceHostFactory2(), typeof(BizMsgService)));
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes();
        }
    }
}
