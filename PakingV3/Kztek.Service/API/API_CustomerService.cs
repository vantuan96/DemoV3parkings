using Kztek.Data.Infrastructure;
using Kztek.Data.Repository;
using Kztek.Model.CustomModel;
using Kztek.Model.Models;
using Kztek.Web.Core.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kztek.Service.API
{
    public interface IAPI_CustomerService
    {
        IEnumerable<tblCustomer> GetAll();
        IEnumerable<tblCustomer_API3rd> GetTop10(string key);
       // IEnumerable<tblCustomerGroup> GetAll_CustomerGroup();
        tblCustomer_API3rd GetByNameOrAdd(string name, string add);
        MessageReport Create(tblCustomer obj);
    }

    public class API_CustomerService : IAPI_CustomerService
    {
        private ItblCustomerRepository _tblCustomerRepository;

        private IUnitOfWork _UnitOfWork;
        public API_CustomerService(ItblCustomerRepository _tblCustomerRepository, IUnitOfWork _UnitOfWork)
        {
            this._tblCustomerRepository = _tblCustomerRepository;
            this._UnitOfWork = _UnitOfWork;
        }
        private void Save()
        {
            _UnitOfWork.Commit();
        }
        /// <summary>
        /// get all card group
        /// </summary>
        /// <returns></returns>
        public IEnumerable<tblCustomer> GetAll()
        {
            var query = from n in _tblCustomerRepository.Table
                        select n;

            //if (!string.IsNullOrEmpty(AuthCustomerIds))
            //{
            //    var list = AuthCustomerIds.Split(';');
            //    query = query.Where(n => list.Contains(n.CustomerID.ToString()));
            //}

            return query;
        }



        public IEnumerable<tblCustomer_API3rd> GetTop10(string key)
        {
            var query = from c in _tblCustomerRepository.Table
                        //join p in _tblCustomerGroupRepository.Table on c.CustomerGroupID equals p.CustomerGroupID.ToString()
                        orderby (c.SortOrder)
                        select new tblCustomer_API3rd()
                        {
                            CustomerID = c.CustomerID.ToString(),
                            CustomerCode = c.CustomerCode,
                            CustomerGroupID = c.CustomerGroupID,
                           // CustomerGroupName = p.CustomerGroupName,
                            IDNumber = c.IDNumber,
                            Mobile = c.Mobile,
                            Address = c.Address,
                            CustomerName = c.CustomerName
                        };

            if (!string.IsNullOrWhiteSpace(key))
            {
                query = query.Where(n => n.CustomerName.Contains(key) || n.CustomerCode.Contains(key) || n.IDNumber.Contains(key) || n.Mobile.Contains(key));
            }
            query = query.Take(10);

            return query;
        }

        public tblCustomer_API3rd GetByNameOrAdd(string name, string add)
        {
            var query = from c in _tblCustomerRepository.Table
                        where c.Inactive == false && c.CustomerName.Contains(name) 
                        orderby (c.SortOrder)
                        select new tblCustomer_API3rd()
                        {
                            CustomerID = c.CustomerID.ToString(),
                            CustomerCode = c.CustomerCode,
                            CustomerGroupID = c.CustomerGroupID,
                            IDNumber = c.IDNumber,
                            Mobile = c.Mobile,
                            Address = c.Address,
                            CustomerName = c.CustomerName
                        };

            if (!String.IsNullOrWhiteSpace(add))
                query = query.Where(n => n.Address.Contains(add));
            return query.FirstOrDefault();
        }

        public MessageReport Create(tblCustomer obj)
        {
            var re = new MessageReport();
            re.Message = "Error";
            re.isSuccess = false;

            try
            {
                _tblCustomerRepository.Add(obj);

                Save();

                re.Message = "Tạo mới khách hàng thành công";
                re.isSuccess = true;
            }
            catch (Exception ex)
            {
                re.Message = ex.Message;
                re.isSuccess = false;
            }

            return re;
        }
    }
}

