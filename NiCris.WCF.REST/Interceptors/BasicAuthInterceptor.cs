using System;
using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.Net;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;
using System.Web.Security;
using Microsoft.ServiceModel.Web;

namespace NiCris.WCF.REST.Interceptors
{
    public class BasicAuthInterceptor : RequestInterceptor
    {
        MembershipProvider provider;
        string realm;

        public BasicAuthInterceptor(MembershipProvider provider, string realm)
            : base(false)
        {
            this.provider = provider;
            this.realm = realm;
        }

        protected string Realm
        {
            get { return realm; }
        }

        protected MembershipProvider Provider
        {
            get { return provider; }
        }

        public override void ProcessRequest(ref RequestContext requestContext)
        {
            string[] credentials = ExtractCredentials(requestContext.RequestMessage);

            if (credentials.Length > 0 && AuthenticateUser(credentials[0], credentials[1]))
            {
                // It's all good...
                InitializeSecurityContext(requestContext.RequestMessage, credentials[0]);
            }
            else
            {
                Message reply = Message.CreateMessage(MessageVersion.None, null);
                HttpResponseMessageProperty responseProperty = new HttpResponseMessageProperty()
                {
                    StatusCode = HttpStatusCode.Unauthorized
                };

                responseProperty.Headers.Add("WWW-Authenticate", String.Format("Basic realm=\"{0}\"", Realm));

                reply.Properties[HttpResponseMessageProperty.Name] = responseProperty;
                requestContext.Reply(reply);
                requestContext = null;
            }
        }

        private bool AuthenticateUser(string userName, string password)
        {
            if (Provider.ValidateUser(userName, password))
                return true;
            return false;
        }

        private string[] ExtractCredentials(Message reqMessage)
        {
            var request = (HttpRequestMessageProperty)reqMessage.Properties[HttpRequestMessageProperty.Name];
            string authHeader = request.Headers["Authorization"];

            if (authHeader != null && authHeader.StartsWith("Basic"))
            {
                string encodedUserPass = authHeader.Substring(6).Trim();

                Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                string userPass = encoding.GetString(Convert.FromBase64String(encodedUserPass));
                int separator = userPass.IndexOf(':');

                string[] credentials = new string[2];
                credentials[0] = userPass.Substring(0, separator);
                credentials[1] = userPass.Substring(separator + 1);

                return credentials;
            }

            return new string[] { };
        }

        private void InitializeSecurityContext(Message request, string userName)
        {
            var principal = new GenericPrincipal(new GenericIdentity(userName), new string[] { });
            List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>();
            policies.Add(new PrincipalAuthorizationPolicy(principal));

            var secContext = new ServiceSecurityContext(policies.AsReadOnly());

            if (request.Properties.Security != null)
                request.Properties.Security.ServiceSecurityContext = secContext;
            else
            {
                request.Properties.Security = new SecurityMessageProperty()
                {
                    ServiceSecurityContext = secContext
                };
            }
        }

    }
}