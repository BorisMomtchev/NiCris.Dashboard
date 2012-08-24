using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace NiCris.WCF.REST.ViewModels
{
    [CollectionDataContract(Namespace = "", Name = "BizMsgList")]
    public class BizMsgList : List<NiCris.BusinessObjects.BizMsg>
    {
    }
}