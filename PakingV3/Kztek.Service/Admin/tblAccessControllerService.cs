using Kztek.Data.Infrastructure;
using Kztek.Data.Repository;
using Kztek.Model.CustomModel;
using Kztek.Model.Models;
using Kztek.Web.Core.Functions;
using Kztek.Web.Core.Models;
using Kztek.Web.Core.Helpers;
using PagedList;
using System;
using System.Linq;
using System.Collections.Generic;
namespace Kztek.Service.Admin
{
    public interface ItblAccessControllerService
    {
        IQueryable<tblAccessController> GetAll();
        IQueryable<tblAccessController> GetAllActive();
        IQueryable<tblAccessController> GetAllByListId(List<string> Ids);
        IPagedList<tblAccessController> GetAllPagingByFirst_AccessController(string key, string line, string groupControllerId ,int pageNumber, int pageSize);
        IPagedList<tblAccessController> GetAllPagingByFirst(string key, string computerids, string line, string groupControllerId, int pageNumber, int pageSize);

        IEnumerable<tblAccessController> GetAllByFirst(string key, string computerids, string line);

        IEnumerable<tblAccessController> GetAllActiveByListId(string ids);

        IEnumerable<tblAccessController> GetAllActiveByListId(List<string> ids);

        tblAccessController GetById(string id);
        tblAccessController GetById(Guid id);

        MessageReport Create(tblAccessController obj);
        MessageReport Update(tblAccessController obj);
        MessageReport DeleteById(string id);
    }

    public class tblAccessControllerService : ItblAccessControllerService
    {
        private readonly ItblAccessControllerRepository _tblAccessControllerRepository;
        private readonly ItblAccessLineService _tblAccessLineService;
        private readonly IUnitOfWork _UnitOfWork;
        public tblAccessControllerService(ItblAccessControllerRepository _tblAccessControllerRepository, ItblAccessLineService _tblAccessLineService, IUnitOfWork _UnitOfWork)
        {
            this._tblAccessControllerRepository = _tblAccessControllerRepository;
            this._tblAccessLineService = _tblAccessLineService;
            this._UnitOfWork = _UnitOfWork;
        }

        public IQueryable<tblAccessController> GetAll()
        {
            var query = from n in _tblAccessControllerRepository.Table select n;
            return query;
        }
        public IPagedList<tblAccessController> GetAllPagingByFirst_AccessController(string key, string line, string groupControllerId, int pageNumber, int pageSize)
        {
            var query = from n in _tblAccessControllerRepository.Table
                        select n;

            if (!string.IsNullOrWhiteSpace(key))
            {
                query = query.Where(n => n.ControllerName.Contains(key));
            }

            if (!string.IsNullOrWhiteSpace(line))
            {
                query = query.Where(n => n.LineID == line);
            }

            if (!string.IsNullOrWhiteSpace(groupControllerId))
            {
                query = query.Where(n => n.ControllerGroupId == groupControllerId);
            }

            var list = new PagedList<tblAccessController>(query.OrderBy(n => n.ControllerID), pageNumber, pageSize);
            return list;
        }

        public tblAccessController GetById(string id)
        {
            return _tblAccessControllerRepository.GetById(id);
        }

        public MessageReport Create(tblAccessController obj)
        {
            MessageReport report;
            try
            {
                _tblAccessControllerRepository.Add(obj);
                Save();
                report = new MessageReport(true, "Thêm thành công");
            }
            catch (Exception ex)
            {
                report = new MessageReport(false, ex.InnerException != null ? ex.InnerException.ToString() : ex.Message);
            }
            return report;
        }

        public MessageReport Update(tblAccessController obj)
        {
            MessageReport report;

            try
            {
                _tblAccessControllerRepository.Update(obj);
                Save();
                report = new MessageReport(true, "Cập nhật thành công");
            }
            catch (Exception ex)
            {
                report = new MessageReport(false, ex.InnerException != null ? ex.InnerException.ToString() : ex.Message);
            }

            return report;
        }

        public MessageReport DeleteById(string id)
        {
            MessageReport report;
            try
            {
                var obj = _tblAccessControllerRepository.GetById(Guid.Parse(id));
                if (obj != null)
                {
                    _tblAccessControllerRepository.Delete(obj);
                    Save();
                    report = new MessageReport(true, "Xóa thông tin thành công");
                }
                else
                {
                    report = new MessageReport(false, "Thông tin này không tồn tại");
                }
            }
            catch (Exception ex)
            {
                report = new MessageReport(false, ex.InnerException != null ? ex.InnerException.ToString() : ex.Message);
            }
            return report;
        }
        
        //Save change
        public void Save()
        {
            _UnitOfWork.Commit();
        }

        public tblAccessController GetById(Guid id)
        {
            return _tblAccessControllerRepository.GetById(id);
        }

        public IQueryable<tblAccessController> GetAllActive()
        {
            var query = from n in _tblAccessControllerRepository.Table
                        where n.Inactive == false
                        select n;
            return query;
        }

        public IQueryable<tblAccessController> GetAllByListId(List<string> Ids)
        {
            var query = from n in _tblAccessControllerRepository.Table
                        where n.Inactive == false
                        select n;

            if (Ids != null && Ids.Any())
            {
                query = query.Where(n => Ids.Contains(n.ControllerID.ToString()));
            }

            return query;
        }

        public IPagedList<tblAccessController> GetAllPagingByFirst(string key, string computerids, string line, string groupControllerId, int pageNumber, int pageSize)
        {
            var query = from n in _tblAccessControllerRepository.Table
                        select n;

            if (!string.IsNullOrWhiteSpace(key))
            {
                query = query.Where(n => n.ControllerName.Contains(key));
            }

            if (!string.IsNullOrWhiteSpace(computerids))
            {
                var lines = _tblAccessLineService.GetAllActiveByListPC(computerids);
                if (lines.Any())
                {
                    var str = "";
                    foreach (var item in lines)
                    {
                        str += item.LineID;
                    }

                    query = query.Where(n => str.Contains(n.LineID));
                }
                else
                {
                    var str = "NULL";
                    query = query.Where(n => str.Contains(n.LineID));
                }
            }

            if (!string.IsNullOrEmpty(line) && !line.Equals("0"))
            {
                query = query.Where(n => n.LineID == line);
            }

            if (!string.IsNullOrWhiteSpace(groupControllerId))
            {
                query = query.Where(n => n.ControllerGroupId == groupControllerId);
            }

            var list = new PagedList<tblAccessController>(query.OrderBy(n => n.LineID), pageNumber, pageSize);
            return list;
        }

        public IEnumerable<tblAccessController> GetAllActiveByListId(string ids)
        {
            var query = from n in _tblAccessControllerRepository.Table
                        where n.Inactive == false && ids.Contains(n.ControllerID.ToString())
                        select n;

            return query;
        }

        public IEnumerable<tblAccessController> GetAllActiveByListId(List<string> ids)
        {
            var query = from n in _tblAccessControllerRepository.Table
                        where n.Inactive == false && ids.Contains(n.ControllerID.ToString())
                        select n;

            return query;
        }

        public IEnumerable<tblAccessController> GetAllByFirst(string key, string computerids, string line)
        {
            var query = from n in _tblAccessControllerRepository.Table
                        select n;

            if (!string.IsNullOrWhiteSpace(key))
            {
                query = query.Where(n => n.ControllerName.Contains(key));
            }

            if (!string.IsNullOrWhiteSpace(computerids))
            {
                var lines = _tblAccessLineService.GetAllActiveByListPC(computerids);
                if (lines.Any())
                {
                    var str = "";
                    foreach (var item in lines)
                    {
                        str += item.LineID;
                    }

                    query = query.Where(n => str.Contains(n.LineID));
                }
            }

            return query;
        }
    }
}
