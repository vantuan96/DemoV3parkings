using Kztek.Model.Models;
using Kztek.Service.Admin;
using Kztek.Web.Attributes;
using Kztek.Web.Core.Functions;
using Kztek.Web.Core.Models;
using Kztek.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kztek.Web.Areas.Access.Controllers
{
    public class tblControllerController : Controller
    {
        #region Khai báo services

        private ItblAccessControllerService _tblAccessControllerService;
        private ItblAccessLineService _tblAccessLineService;
        private ItblAccessControllerGroupService _tblAccessControllerGroupService;
        private static string url = "";

        public tblControllerController(ItblAccessControllerService _tblAccessControllerService, ItblAccessLineService _tblAccessLineService, ItblAccessControllerGroupService _tblAccessControllerGroupService)
        {
            this._tblAccessControllerService = _tblAccessControllerService;
            this._tblAccessLineService = _tblAccessLineService;
            this._tblAccessControllerGroupService = _tblAccessControllerGroupService;
        }

        #endregion Khai báo services

        #region Danh sách

        /// <summary>
        /// Danh sách bộ điều khiển
        /// </summary>
        /// <modified>
        /// Author              Date            Comments
        /// TrungNQ             01/09/2017      Tạo mới
        /// </modified>
        /// <param name="key">Từ khóa</param>
        /// <param name="pc">Id máy tính</param>
        /// <param name="page">Trang hiện tại</param>
        /// <returns></returns>
        [CheckSessionLogin]
        public ActionResult Index(string key = "", string line = "", string GroupControllerId = "", int page = 1)
        {
            //var totalPage = 0;
            //var totalItem = 0;
            var pageSize = 20;

            var list = _tblAccessControllerService.GetAllPagingByFirst_AccessController(key, line, GroupControllerId, page, pageSize);
            foreach (var item in list)
            {
                if (!String.IsNullOrEmpty(item.ControllerGroupId))
                {
                    var ControllerGroupName = GetControllerGroupList().Where(n => n.Id == item.ControllerGroupId).FirstOrDefault().Name;
                    item.ControllerGroupId = ControllerGroupName;
                }
            }
            var gridModel = PageModelCustom<tblAccessController>.GetPage(list, page, pageSize);

            ViewBag.KeyWord = key;
            ViewBag.Lines = GetLineList();
            ViewBag.LineID = line;
            ViewBag.GroupController = GetControllerGroupList();
            ViewBag.GroupControllerId = GroupControllerId;

            url = Request.Url.PathAndQuery;

            return View(gridModel);
        }

        #endregion Danh sách

        #region Thêm mới

        /// <summary>
        /// Giao diện thêm mới
        /// </summary>
        /// <modified>
        /// Author              Date            Comments
        /// TrungNQ             01/09/2017      Tạo mới
        /// </modified>
        /// <returns></returns>
        [CheckSessionLogin]
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Lines = GetLineList();
            ViewBag.GroupController = GetControllerGroupList();

            ViewBag.urlValue = url ?? Request.Url.PathAndQuery;

            return View();
        }

        /// <summary>
        /// Thực hiện thêm mới
        /// </summary>
        /// <modified>
        /// Author              Date            Comments
        /// TrungNQ             01/09/2017      Tạo mới
        /// </modified>
        /// <param name="obj">Đối tượng</param>
        /// <param name="SaveAndCountinue">Thêm liên tục hay không?</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(tblAccessController obj, bool SaveAndCountinue = false)
        {
            //Danh sách sử dụng
            ViewBag.Lines = GetLineList();
            ViewBag.GroupController = GetControllerGroupList();

            ViewBag.urlValue = url ?? Request.Url.PathAndQuery;

            //Kiểm tra
            if (!ModelState.IsValid)
            {
                return View(obj);
            }

            //Gán giá trị
            obj.ControllerID = Guid.NewGuid();

            //Thực hiện thêm mới
            var result = _tblAccessControllerService.Create(obj);
            if (result.isSuccess)
            {
                WriteLog.Write(result, GetCurrentUser.GetUser(), obj.ControllerID.ToString(), obj.ControllerName, "tblAccessController", ConstField.AccessControlCode, ActionConfigO.Create);

                if (SaveAndCountinue)
                {
                    TempData["Success"] = result.Message;
                    return RedirectToAction("Create");
                }

                return Redirect(url);
            }
            else
            {
                return View(obj);
            }
        }

        #endregion Thêm mới

        #region Cập nhật

        /// <summary>
        /// Giao diện cập nhật
        /// </summary>
        /// <modified>
        /// Author              Date            Comments
        /// TrungNQ             01/09/2017      Tạo mới
        /// </modified>
        /// <param name="id">Id bản ghi</param>
        /// <param name="pageNumber">Trang hiện tại</param>
        /// <returns></returns>
        [CheckSessionLogin]
        [HttpGet]
        public ActionResult Update(string id)
        {
            ViewBag.Lines = GetLineList();
            ViewBag.GroupController = GetControllerGroupList();

            ViewBag.urlValue = url ?? Request.Url.PathAndQuery;

            var obj = _tblAccessControllerService.GetById(Guid.Parse(id));


            return View(obj);
        }

        /// <summary>
        /// Thực hiện cập nhật
        /// </summary>
        /// <modified>
        /// Author              Date            Comments
        /// TrungNQ             01/09/2017      Tạo mới
        /// </modified>
        /// <param name="obj">Đối tượng</param>
        /// <param name="objId">Id bản ghi</param>
        /// <param name="pageNumber">Trang hiện tại</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Update(tblAccessController obj)
        {
            //Danh sách sử dụng
            ViewBag.Lines = GetLineList();
            ViewBag.GroupController = GetControllerGroupList();

            ViewBag.urlValue = url ?? Request.Url.PathAndQuery;

            //Kiểm tra
            var oldObj = _tblAccessControllerService.GetById(obj.ControllerID);
            if (oldObj == null)
            {
                ViewBag.Error = "Bản ghi không tồn tại";
                return View(obj);
            }

            if (!ModelState.IsValid)
            {
                return View(oldObj);
            }

            //Gán giá trị
            oldObj.ControllerName = obj.ControllerName;
            oldObj.Inactive = obj.Inactive;
            oldObj.LineID = obj.LineID;
            oldObj.Address = obj.Address;
            oldObj.ControllerGroupId = obj.ControllerGroupId;

            //Thực hiện cập nhật
            var result = _tblAccessControllerService.Update(oldObj);
            if (result.isSuccess)
            {
                WriteLog.Write(result, GetCurrentUser.GetUser(), oldObj.ControllerID.ToString(), oldObj.ControllerName, "tblAccessController", ConstField.AccessControlCode, ActionConfigO.Update);

                return Redirect(url);
            }
            else
            {
                ModelState.AddModelError("", result.Message);
                return View(oldObj);
            }
        }

        #endregion Cập nhật

        #region Xóa

        /// <summary>
        /// Xóa
        /// </summary>
        /// <modified>
        /// Author              Date            Comments
        /// TrungNQ             01/09/2017      Tạo mới
        /// </modified>
        /// <param name="id">Id bản ghi</param>
        /// <returns></returns>
        public JsonResult Delete(string id)
        {
            var result = _tblAccessControllerService.DeleteById(id);
            if (result.isSuccess)
            {
                WriteLog.Write(result, GetCurrentUser.GetUser(), id, id, "tblAccessController", ConstField.AccessControlCode, ActionConfigO.Delete);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion Xóa

        private List<tblAccessLine> GetLineList()
        {
            return _tblAccessLineService.GetAllActive().ToList();
        }

        private List<tblAccessControllerGroup> GetControllerGroupList()
        {
            return _tblAccessControllerGroupService.GetAll().OrderBy(n=>n.SortOrder).ToList();
        }
    }
}