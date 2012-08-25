﻿using System.IO;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using Microsoft.ServiceModel.Web;
using NiCris.BusinessObjects;
using NiCris.CoreServices.Interfaces;
using NiCris.CoreServices.Services;
using NiCris.DataAccess.SQL.Repositories;
using NiCris.WCF.REST.ViewModels;
using System.Net;
using System;

namespace NiCris.WCF.REST
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceContract(Namespace = "")]
    public class BizMsgService
    {
        IBizMsgCoreService _bizMsgCoreService;

        public BizMsgService()
        {
            _bizMsgCoreService = new BizMsgCoreService(new BizMsgRepository());
        }

        [WebHelp(Comment = "Gets all BizMsgs.")]
        [WebGet(UriTemplate = "")]
        [OperationContract]
        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        BizMsgList GetAllBizMsgs()
        {
            // Upon exception InternalServerError (500) is returned with Details
            BizMsgList bizMsgList = new BizMsgList();
            bizMsgList.AddRange(_bizMsgCoreService.GetAll());
            return bizMsgList;
        }

        [WebHelp(Comment = "Gets a specific BizMsgs.")]
        [WebGet(UriTemplate = "{id}")]
        [OperationContract]
        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        BizMsg GetBizMsg(string id)
        {
            BizMsg bizMsg = _bizMsgCoreService.GetById(int.Parse(id));
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
            var id = _bizMsgCoreService.Insert(bizMsg);

            var ctx = WebOperationContext.Current.OutgoingResponse;
            ctx.StatusCode = System.Net.HttpStatusCode.Created;

            // Set the Http Location Header
            HttpResponseHeader locationHeader = HttpResponseHeader.Location;
            string locationValue = WebOperationContext.Current.IncomingRequest.GetRequestUri() + id.ToString();
            ctx.Headers.Add(locationHeader, locationValue);
        }

        [WebHelp(Comment = "Updates a BizMsgs.")]
        [WebInvoke(UriTemplate = "{id}", Method = "PUT")]
        [OperationContract]
        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        void UpdateBizMsg(string id, BizMsg bizMsg)
        {
            var bizMsgToUpd = _bizMsgCoreService.GetById(int.Parse(id));

            bizMsgToUpd.Name = bizMsg.Name;
            bizMsgToUpd.Date = bizMsg.Date;
            bizMsgToUpd.User = bizMsg.User;

            bizMsgToUpd.Description = bizMsg.Description;
            bizMsgToUpd.AppId = bizMsg.AppId;
            bizMsgToUpd.ServiceId = bizMsg.ServiceId;
            bizMsgToUpd.StyleId = bizMsg.StyleId;
            bizMsgToUpd.Roles = bizMsg.Roles;

            var updId = _bizMsgCoreService.Update(bizMsgToUpd);

            OutgoingWebResponseContext ctx = WebOperationContext.Current.OutgoingResponse;
            ctx.StatusCode = System.Net.HttpStatusCode.Accepted;

            // Set the Http Location Header
            HttpResponseHeader locationHeader = HttpResponseHeader.Location;
            string locationValue = WebOperationContext.Current.IncomingRequest.GetRequestUri() + id.ToString();
            ctx.Headers.Add(locationHeader, locationValue);
        }

        [WebHelp(Comment = "Deletes an BizMsgs.")]
        [WebInvoke(UriTemplate = "{id}", Method = "DELETE")]
        [OperationContract]
        [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        void DeleteBizMsg(string id)
        {
            BizMsg bizMsg = _bizMsgCoreService.GetById(int.Parse(id));
            _bizMsgCoreService.Delete(bizMsg);
        }

        // *************************************************************

        /*
        [WebHelp(Comment = "Gets all BizMsgs in Json.")]
        [WebGet(UriTemplate = "?json", ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        BizMsgList GetAllBizMsgsJson()
        {
            BizMsgList bizMsgList = new BizMsgList();
            bizMsgList.AddRange(_bizMsgCoreService.GetAll());
            return bizMsgList;
        }
        */

        [WebHelp(Comment = "Gets all BizMsgs as Json stream.")]
        [WebGet(UriTemplate = "?json")]
        [OperationContract]
        Stream GetAllBizMsgsJsonStream()
        {
            BizMsgList bizMsgList = new BizMsgList();
            bizMsgList.AddRange(_bizMsgCoreService.GetAll());

            DataContractJsonSerializer x = new DataContractJsonSerializer(bizMsgList.GetType());
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/json";

            return new AdapterStream((stream) => x.WriteObject(stream, bizMsgList));
        }
    }
}