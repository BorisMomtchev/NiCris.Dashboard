using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using Microsoft.ServiceModel.Web;
using NiCris.BusinessObjects;
using NiCris.CoreServices.Interfaces;
using NiCris.CoreServices.Services;
using NiCris.DataAccess.SQL.Repositories;
using NiCris.WCF.REST.ViewModels;

namespace NiCris.WCF.REST
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceContract(Namespace = "")]
    public class BizMsgService
    {
        IBizMsgCoreService _businessMsgService;

        public BizMsgService()
        {
            _businessMsgService = new BizMsgCoreService(new BizMsgRepository());
        }

        [WebHelp(Comment = "Gets all BizMsgs.")]
        [WebGet(UriTemplate = "")]
        [OperationContract]
        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        BizMsgList GetAllBizMsgs()
        {
            // Upon exception InternalServerError (500) is returned with Details
            BizMsgList bizMsgList = new BizMsgList();
            bizMsgList.AddRange(_businessMsgService.GetAll());
            
            return bizMsgList;
        }

        [WebHelp(Comment = "Gets a specific BizMsgs.")]
        [WebGet(UriTemplate = "/{id}")]
        [OperationContract]
        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        BizMsg GetBizMsg(string id)
        {
            BizMsg bizMsg = _businessMsgService.GetById(int.Parse(id));
            if (bizMsg == null)
            {
                OutgoingWebResponseContext ctx = WebOperationContext.Current.OutgoingResponse;
                ctx.StatusCode = System.Net.HttpStatusCode.NotFound;
            }
            return bizMsg;
        }

        [WebHelp(Comment = "Creates a BizMsgs.")]
        [WebInvoke(UriTemplate = "", Method = "POST")]
        [OperationContract]
        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        void CreateBizMsg(BizMsg bizMsg)
        {
            _businessMsgService.Insert(bizMsg);

            OutgoingWebResponseContext ctx = WebOperationContext.Current.OutgoingResponse;
            ctx.StatusCode = System.Net.HttpStatusCode.Created;
        }


        [WebHelp(Comment = "Deletes an BizMsgs.")]
        [WebInvoke(UriTemplate = "/{id}", Method = "DELETE")]
        [OperationContract]
        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        void DeleteBizMsg(string id)
        {
            BizMsg bizMsg = _businessMsgService.GetById(int.Parse(id));
            _businessMsgService.Delete(bizMsg);
        }
    }
}