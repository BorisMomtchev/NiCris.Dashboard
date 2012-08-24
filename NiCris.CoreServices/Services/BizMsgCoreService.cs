using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NiCris.CoreServices.Interfaces;
using NiCris.DataAccess.Interfaces;
using NiCris.DataAccess.SQL.Repositories;

namespace NiCris.CoreServices.Services
{
    public class BizMsgCoreService : IBizMsgCoreService
    {
        // Fields & Properties
        IBizMsgRepository BusinessMsgRepository { get; set; }

        // C~tors
        public BizMsgCoreService() : this(new BizMsgRepository())
        {
        }

        public BizMsgCoreService(IBizMsgRepository businessMsgRepository)
        {
            BusinessMsgRepository = businessMsgRepository;
        }

        #region ICRUDService<BusinessMsg> Members

        public IList<BusinessObjects.BizMsg> GetAll()
        {
            return BusinessMsgRepository.GetAll().ToList();
        }

        public BusinessObjects.BizMsg GetById(int id)
        {
            return BusinessMsgRepository.GetById(id);
        }

        public int Insert(BusinessObjects.BizMsg model)
        {
            return BusinessMsgRepository.Insert(model);
        }

        public int Update(BusinessObjects.BizMsg model)
        {
            return BusinessMsgRepository.Update(model);
        }

        public void Delete(BusinessObjects.BizMsg model)
        {
            BusinessMsgRepository.Delete(model);
        }

        #endregion
    }
}
