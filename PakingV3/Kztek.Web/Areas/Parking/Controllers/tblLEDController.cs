using Kztek.Model.CustomModel.iParking;
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

namespace Kztek.Web.Areas.Parking.Controllers
{
    public class tblLEDController : Controller
    {
        private ItblLEDService _tblLEDService;
        private ItblPCService _tblPCService;

        public tblLEDController(ItblLEDService _tblLEDService, ItblPCService _tblPCService)
        {
            this._tblLEDService = _tblLEDService;
            this._tblPCService = _tblPCService;
        }

        [CheckSessionLogin]
        [CheckAuthorize]
        public ActionResult Index(string key, string pc, int page = 1, string group = "", string selectedId = "")
        {
            var pageSize = 20;

            var list = _tblLEDService.GetAllCustomPagingByFirst(key, pc, page, pageSize);

            var gridModel = PageModelCustom<tblLEDCustomViewModel>.GetPage(list, page, pageSize);

            ViewBag.PCs = GetPCList();

            ViewBag.keyValue = key;
            ViewBag.pcValue = pc;
            ViewBag.groupValue = group;
            ViewBag.selectedIdValue = selectedId;

            return View(gridModel);
        }

        [CheckSessionLogin]
        [CheckAuthorize]
        [HttpGet]
        public ActionResult Create(string key, string pc, string group = "")
        {
            ViewBag.PCs = GetPCList();
            ViewBag.SideIndex = FunctionHelper.HubList1();
            ViewBag.LedType = FunctionHelper.LEDType1();

            ViewBag.keyValue = key;
            ViewBag.pcValue = pc;
            ViewBag.groupValue = group;

            return View();
        }

        [HttpPost]
        public ActionResult Create(tblLEDView obj, string key, string pc, string group = "", bool SaveAndCountinue = false)
        {
            ViewBag.PCs = GetPCList();
            ViewBag.SideIndex = FunctionHelper.HubList1();
            ViewBag.LedType = FunctionHelper.LEDType1();

            ViewBag.keyValue = key;
            ViewBag.pcValue = pc;
            ViewBag.groupValue = group;

            if (!ModelState.IsValid)
            {
                return View(obj);
            }

            if (string.IsNullOrWhiteSpace(obj.LEDName) || (obj.Address <= 0 || obj.Address == null) ||  obj.SideIndex == null)
            {
                if (string.IsNullOrWhiteSpace(obj.LEDName))
                {
                    ModelState.AddModelError("LEDName", FunctionHelper.GetLocalizeDictionary("Home", "notification")["LEDName"]);

                }
                if (obj.Address <= 0 || obj.Address == null)
                {
                    ModelState.AddModelError("Address", FunctionHelper.GetLocalizeDictionary("Home", "notification")["addIp"]);
                }
                if (obj.SideIndex == null)
                {
                    ModelState.AddModelError("SideIndex", FunctionHelper.GetLocalizeDictionary("Home", "notification")["interface"]);
                }
                return View(obj);
            }

            var existed = _tblLEDService.GetByName(obj.LEDName);
            if (existed != null)
            {
                ModelState.AddModelError("LEDName", FunctionHelper.GetLocalizeDictionary("Home", "notification")["LED_Name_already_exists"]);
                return View(obj);
            }

            //Thực hiện thêm mới
            tblLED objCreate = new tblLED();
            objCreate.LEDName = obj.LEDName;
            objCreate.PCID = obj.PCID;
            objCreate.Comport = obj.Comport;
            objCreate.Baudrate = obj.Baudrate;
            objCreate.SideIndex = Convert.ToInt32(obj.SideIndex);
            objCreate.Address = Convert.ToInt32(obj.Address);
            objCreate.LedType = obj.LedType;
            objCreate.EnableLED = obj.EnableLED;

            var result = _tblLEDService.Create(objCreate);
            if (result.isSuccess)
            {
                WriteLog.Write(result, GetCurrentUser.GetUser(), obj.LEDID.ToString(), obj.LEDName, "tblLED", ConstField.ParkingCode, ActionConfigO.Create);

                if (SaveAndCountinue)
                {
                    TempData["Success"] = result.Message;
                    return RedirectToAction("Create", new { group = group, key = key, pc = pc, selectedId = obj.LEDID });
                }

                return RedirectToAction("Index", new { group = group, key = key, pc = pc, selectedId = obj.LEDID });
            }
            else
            {
                return View(obj);
            }
        }

        [CheckSessionLogin]
        [CheckAuthorize]
        [HttpGet]
        public ActionResult Update(string id, int page = 1, string key = "", string pc = "", string group = "")
        {
            ViewBag.PCs = GetPCList();
            ViewBag.SideIndex = FunctionHelper.HubList1();
            ViewBag.LedType = FunctionHelper.LEDType1();

            ViewBag.keyValue = key;
            ViewBag.pcValue = pc;
            ViewBag.groupValue = group;

            ViewBag.PN = page;

            var obj = _tblLEDService.GetById(Convert.ToInt32(id));

            tblLEDView objCreate = new tblLEDView();
            objCreate.LEDID = obj.LEDID;
            objCreate.LEDName = obj.LEDName;
            objCreate.PCID = obj.PCID;
            objCreate.Comport = obj.Comport;
            objCreate.Baudrate = obj.Baudrate;
            objCreate.SideIndex = Convert.ToInt32(obj.SideIndex);
            objCreate.Address = Convert.ToInt32(obj.Address);
            objCreate.LedType = obj.LedType;
            objCreate.EnableLED = obj.EnableLED;

            return View(objCreate);
        }

        [HttpPost]
        public ActionResult Update(tblLEDView obj, int page = 1, string key = "", string pc = "", string group = "")
        {
            ViewBag.PCs = GetPCList();
            ViewBag.SideIndex = FunctionHelper.HubList1();
            ViewBag.LedType = FunctionHelper.LEDType1();

            ViewBag.keyValue = key;
            ViewBag.pcValue = pc;
            ViewBag.groupValue = group;

            ViewBag.PN = page;

            var oldObj = _tblLEDService.GetById(obj.LEDID);
            if (oldObj == null)
            {
                ViewBag.Error = FunctionHelper.GetLocalizeDictionary("Home", "notification")["record_does_not_exist"];
                return View(obj);
            }

            if (string.IsNullOrWhiteSpace(obj.LEDName) || (obj.Address <= 0 || obj.Address == null) ||  obj.SideIndex == null)
            {
                if (string.IsNullOrWhiteSpace(obj.LEDName))
                {
                    ModelState.AddModelError("LEDName", FunctionHelper.GetLocalizeDictionary("Home", "notification")["LEDName"]);

                }
                if (obj.Address <= 0 || obj.Address == null)
                {
                    ModelState.AddModelError("Address", FunctionHelper.GetLocalizeDictionary("Home", "notification")["addIp"]);
                }
                if (obj.SideIndex == null)
                {
                    ModelState.AddModelError("SideIndex", FunctionHelper.GetLocalizeDictionary("Home", "notification")["interface"]);
                }
                return View(obj);
            }

            var objExisted = _tblLEDService.GetByName_Id(obj.LEDName, obj.LEDID.ToString());
            if (objExisted != null)
            {
                ViewBag.Error = FunctionHelper.GetLocalizeDictionary("Home", "notification")["LED_Name_already_exists"];
                return View(oldObj);
            }

            //Gán giá trị
            oldObj.SideIndex  = Convert.ToInt32(obj.SideIndex);
            oldObj.LedType = obj.LedType;
            oldObj.Address = Convert.ToInt32(obj.Address);
            oldObj.Baudrate = obj.Baudrate;
            oldObj.Comport = obj.Comport;
            oldObj.EnableLED = obj.EnableLED;
            oldObj.LEDName = obj.LEDName;

            //Thực hiện cập nhật
            var result = _tblLEDService.Update(oldObj);
            if (result.isSuccess)
            {
                WriteLog.Write(result, GetCurrentUser.GetUser(), obj.LEDID.ToString(), obj.LEDName, "tblLED", ConstField.ParkingCode, ActionConfigO.Update);

                return RedirectToAction("Index", new { group = group, page = page, key = key, pc = pc, selectedId = obj.LEDID });
            }
            else
            {
                ModelState.AddModelError("", result.Message);
                return View(oldObj);
            }
        }

        public JsonResult Delete(string id)
        {
            var obj = new tblLED();

            var result = _tblLEDService.DeleteById(id, ref obj);
            if (result.isSuccess)
            {
                WriteLog.Write(result, GetCurrentUser.GetUser(), obj.LEDID.ToString(), obj.LEDName, "tblLED", ConstField.ParkingCode, ActionConfigO.Delete);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private List<tblPC> GetPCList()
        {
            return _tblPCService.GetAllActive().ToList();
        }
    }
}