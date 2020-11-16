using Kztek.Data.Infrastructure;
using Kztek.Data.Repository;
using Kztek.Data.SqlHelper;
using Kztek.Model.CustomModel;
using Kztek.Model.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kztek.Web.Core.Functions;
using Kztek.Data.AccessEvent.SqlHelper;

namespace Kztek.Service.API
{
    public interface IAPI_tblCardService
    {
        IEnumerable<tblCard> GetAll();

        tblCard GetById(Guid id);

        MessageReport Create(tblCard obj);

        MessageReport Update(tblCard obj);

        MessageReport DeleteById(string id);

        tblCard GetCardByCardNumberOrCardNo(string key);
        tblCard GetByCardNumber_Id(string cardnumber);
    }

    public class API_tblCardService : IAPI_tblCardService
    {
        private ItblCardRepository _tblCardRepository;
        private ItblCardGroupRepository _tblCardGroupRepository;
        private IUnitOfWork _UnitOfWork;

        public API_tblCardService(ItblCardRepository _tblCardRepository, ItblCardGroupRepository _tblCardGroupRepository, ItblCustomerRepository _tblCustomerRepository, ItblCustomerGroupRepository _tblCustomerGroupRepository, ItblAccessLevelRepository _tblAccessLevelRepository, IUnitOfWork _UnitOfWork)
        {
            this._tblCardRepository = _tblCardRepository;
            this._tblCardGroupRepository = _tblCardGroupRepository;
            this._UnitOfWork = _UnitOfWork;
        }

        private void Save()
        {
            _UnitOfWork.Commit();
        }

        public tblCard GetById(Guid id)
        {
            return _tblCardRepository.GetById(id);
        }

        public MessageReport Update(tblCard obj)
        {
            var re = new MessageReport();
            re.Message = "Error";
            re.isSuccess = false;

            try
            {
                _tblCardRepository.Update(obj);

                Save();

                re.Message = FunctionHelper.GetLocalizeDictionary("Home", "notification")["updateSuccess"];
                re.isSuccess = true;
            }
            catch (Exception ex)
            {
                re.Message = ex.Message;
                re.isSuccess = false;
            }

            return re;
        }

        public MessageReport Create(tblCard obj)
        {
            var re = new MessageReport();
            re.Message = "Error";
            re.isSuccess = false;

            try
            {
                _tblCardRepository.Add(obj);

                Save();

                re.Message = FunctionHelper.GetLocalizeDictionary("Home", "notification")["addSuccess"];
                re.isSuccess = true;
            }
            catch (Exception ex)
            {
                re.Message = ex.Message;
                re.isSuccess = false;
            }

            return re;
        }

        public MessageReport DeleteById(string id)
        {

            var re = new MessageReport();
            re.Message = "Error";
            re.isSuccess = false;

            try
            {
                var obj = GetById(Guid.Parse(id));
                if (obj != null)
                {
                    obj.IsDelete = true;

                    _tblCardRepository.Update(obj);

                    Save();
                }

                re.Message = FunctionHelper.GetLocalizeDictionary("Home", "notification")["DeleteSuccess"];
                re.isSuccess = true;
            }
            catch (Exception ex)
            {
                re.Message = ex.Message;
                re.isSuccess = false;
            }

            return re;
        }

        public IEnumerable<tblCard> GetAll()
        {
            var query = from n in _tblCardRepository.Table
                        where n.IsDelete == false
                        select n;

            return query;
        }

        public tblCard GetCardByCardNumberOrCardNo(string key)
        {
            var query = from n in _tblCardRepository.Table
                        where /*n.IsLock == false &&*/ (n.CardNumber.Contains(key) || n.CardNo.Contains(key)) && n.IsDelete == false
                        //orderby n.CardNo ascending
                        select n;
            return query.FirstOrDefault();
        }


        public tblCard GetByCardNumber_Id(string cardnumber)
        {
            var query = from n in _tblCardRepository.Table
                        where n.IsLock == false && n.CardNumber.Contains(cardnumber) && n.IsDelete == false
                        select n;

            return query.FirstOrDefault();
        }

    }
}

