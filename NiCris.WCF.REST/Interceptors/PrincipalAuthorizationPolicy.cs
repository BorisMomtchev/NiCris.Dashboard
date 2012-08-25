using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Security.Principal;

namespace NiCris.WCF.REST.Interceptors
{
    public class PrincipalAuthorizationPolicy : IAuthorizationPolicy
    {
        IPrincipal user;
        string id = Guid.NewGuid().ToString();

        public PrincipalAuthorizationPolicy(IPrincipal user)
        {
            this.user = user;
        }

        public ClaimSet Issuer
        {
            get { return ClaimSet.System; }
        }

        public string Id
        {
            get { return this.id; }
        }

        public bool Evaluate(EvaluationContext evaluationContext, ref object state)
        {
            Claim claim = Claim.CreateNameClaim(user.Identity.Name);
            evaluationContext.AddClaimSet(this, new DefaultClaimSet(claim));

            evaluationContext.Properties["Identities"] =
                new List<IIdentity>(new IIdentity[] { user.Identity });

            evaluationContext.Properties["Principal"] = user;

            return true;
        }
    }
}