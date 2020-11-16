using Kztek.Model.CustomModel;
using Kztek.Model.Models;
using Kztek.Service.Admin;
using Kztek.Service.Admin.Event;
using Kztek.Web.Core.Extensions;
using Kztek.Web.Core.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Kztek.Web.Core.Helpers;
using System.Configuration;

namespace Kztek.Web.Areas.Parking.Controllers
{
    public class tblCardsController : Controller
    {

        #region DI
        private ItblSystemConfigService _tblSystemConfigService;
        private ItblCustomerService _tblCustomerService;
        private ItblCustomerGroupService _tblCustomerGroupservice;
        private ItblCardGroupService _cardGroupService;
        private ItblCardService _itblCardService;
        private ItblCardEventService _tblCardEvenService;
        public tblCardsController(ItblCardService _itblCardService, ItblSystemConfigService _tblSystemConfigService, ItblCustomerGroupService _tblCustomerGroupservice, ItblCardGroupService _cardGroupService, ItblCardEventService _tblCardEvenService, ItblCustomerService _tblCustomerService)
        {
            this._itblCardService = _itblCardService;
            this._tblSystemConfigService = _tblSystemConfigService;
            this._tblCustomerGroupservice = _tblCustomerGroupservice;
            this._cardGroupService = _cardGroupService;
            this._cardGroupService = _cardGroupService;
            this._tblCardEvenService = _tblCardEvenService;
            this._tblCustomerService = _tblCustomerService;
        }
        #endregion
        // GET: Parking/tblCards
        #region Danh sách thẻ
        public ActionResult Index(string key = "", string fromdate = "", string todate = "", bool desc = true, string columnquery = "ImportDate", string customerGroup = "", string isCheckByTime = "0", int page = 1, string cardgroup = "")
        {
            int pageSize = 20;
            int totalItem = 0;
            var str = new List<string>();
            GetListChildCustomer(str, customerGroup);

            if (string.IsNullOrWhiteSpace(fromdate))
            {
                fromdate = DateTime.Now.ToString("dd/MM/yyy");
            }
            if (string.IsNullOrWhiteSpace(todate))
            {
                todate = DateTime.Now.ToString("dd/MM/yyy");

            }
            var systemConfig = _tblSystemConfigService.GetDefault();
            var list = _itblCardService.GetAllCartPagingByFirstTSQL(key, fromdate, desc, columnquery, todate, cardgroup, str, isCheckByTime, page, pageSize, ref totalItem);


            var girdMode = PageModelCustom<tblCardCustomViewModel>.GetPage(list, page, pageSize, totalItem);
            //if (systemConfig.FeeName.Contains("TRANSERCO"))
            //{
            //    ViewBag.ListMoney = _itblCardService.GetMoneyByCardNumber();
            //}
            ViewBag.ListMoney = _itblCardService.GetMoneyByCardNumber();
            // ViewBag.System = systemConfig;
            //ViewBag.fromDate = fromdate;
            //ViewBag.toDate = !string.IsNullOrWhiteSpace(todate) ? Convert.ToDateTime(todate).ToString("dd/MM/yyyy HH:mm:59") : DateTime.Now.ToString("dd/MM/yyyy 23:59:59");
            ViewBag.fromdateValue = fromdate;
            ViewBag.todateValue = todate;
            ViewBag.KeyWord = key;
            ViewBag.GroupCustomers = GetMenuList();
            ViewBag.CustomerId = customerGroup;
            ViewBag.CardGroupDT = GetListCardGroups().ToDataTableNullable();
            ViewBag.CardGroupid = cardgroup;
            ViewBag.isCheckByTimeValue = isCheckByTime;
            ViewBag.ColumnQuery = columnquery;
            ViewBag.Desc = desc;
            // ViewBag.ISTRANSERCO = systemConfig != null ? (systemConfig.FeeName.Contains("TRANSERCO") ? true : false) : false;
            return View(girdMode);
        }
        #endregion
        #region Tạo thẻ mới
        [HttpGet]
        public ActionResult Create(string key, string cardgroups, string customergroups, string active, string fromdate, string todate, string isCheckByTime = "0")
        {

            ViewBag.CardGroups = GetListCardGroup();
            ViewBag.lcustomergroups = GetMenuList();
            ViewBag.keyValue = key;
            ViewBag.cardgroupsValue = cardgroups;
            ViewBag.customergroupsValue = customergroups;
            ViewBag.activeValue = active;
            ViewBag.fromdateValue = fromdate;
            ViewBag.todateValue = todate;
            ViewBag.isCheckByTimeValue = isCheckByTime;
            return View();
        }
        [HttpPost]
        public ActionResult Create(tblCardSubmit tblCard, HttpPostedFileBase FileUpload, string key, string cardgroups, string customergroups, string active, string fromdate, string todate, string isCheckByTime = "0", bool SaveAndCountinue = false)
        {
            ViewBag.CardGroups = GetListCardGroup();
            ViewBag.lcustomergroups = GetMenuList();
            ViewBag.CardGroups = GetListCardGroup();
            ViewBag.lcustomergroups = GetMenuList();
            ViewBag.keyValue = key;
            ViewBag.cardgroupsValue = cardgroups;
            ViewBag.customergroupsValue = customergroups;
            ViewBag.activeValue = active;
            ViewBag.fromdateValue = fromdate;
            ViewBag.todateValue = todate;
            ViewBag.isCheckByTimeValue = isCheckByTime;
            if (!ModelState.IsValid)
            {
                return View(tblCard);
            }
            // valid thẻ
            if ((string.IsNullOrEmpty(tblCard.CardNo) || string.IsNullOrWhiteSpace(tblCard.CardNo)) || (string.IsNullOrWhiteSpace(tblCard.CardNumber) || string.IsNullOrEmpty(tblCard.CardNumber)) || (string.IsNullOrWhiteSpace(tblCard.CardNumber)))
            {
                if ((string.IsNullOrEmpty(tblCard.CardNo) || string.IsNullOrWhiteSpace(tblCard.CardNo)))
                {
                    ModelState.AddModelError("", "Hãy nhập lại mã thẻ");
                }
                if ((string.IsNullOrWhiteSpace(tblCard.CardNumber) || string.IsNullOrEmpty(tblCard.CardNumber)))
                {
                    ModelState.AddModelError("", "Hãy nhập lại Số thẻ ");
                }
                if (string.IsNullOrWhiteSpace(tblCard.CardNumber))
                {
                    ModelState.AddModelError("", "NHập lại ID ");
                }
                return View(tblCard);
            }

            // check mã thẻ có tồn tại k
            var cardNumber = _itblCardService.GetByCardNumber(tblCard.CardNumber.Trim());
            if (cardNumber != null)
            {
                ModelState.AddModelError("CardNumber", "mã thẻ đã tồn tại");
                return View(tblCard);
            }

            // map
            var map = new tblCard()
            {
                CardID = Guid.NewGuid(),
                CardNo = tblCard.CardNo,
                CardNumber = tblCard.CardNumber.Trim(),
                CardGroupID = tblCard.CardGroupID,

                CustomerID = GetOrSetCustomer(tblCard, FileUpload),

             

                AccessLevelID = "",
                ChkRelease = false,
                ImportDate = DateTime.Now,
                DateRegister = Convert.ToDateTime(tblCard.DtpDateRegisted),
                DateRelease = Convert.ToDateTime(tblCard.DtpDateReleased),
                ExpireDate = Convert.ToDateTime(tblCard.DtpDateExpired),
                DateActive = Convert.ToDateTime(tblCard.DtpDateActive),
                Description = !string.IsNullOrWhiteSpace(tblCard.CardDescription) ? tblCard.CardDescription : "",
                IsDelete = false,
                IsLock = tblCard.CardInActive,
                Plate1 = tblCard.Plate1,
                Plate2 = tblCard.Plate2,
                Plate3 = tblCard.Plate3,
                VehicleName1 = tblCard.VehicleName1,
                VehicleName2 = tblCard.VehicleName2,
                VehicleName3 = tblCard.VehicleName3,
                DVT = tblCard.DVT,
                AccessExpireDate = Convert.ToDateTime("2099/12/31"),
                DateCancel = DateTime.Now,

                isAutoCapture = tblCard.IsAutoCapture



            };

            //thực hiện thêm mới
            var result = _itblCardService.Create(map);
            if (result.isSuccess)
            {

                return RedirectToAction("Index", new { key = key, cardgroups = cardgroups, customergroups = customergroups, fromdate = fromdate, todate = todate, active = active, isCheckByTime = isCheckByTime, selectedId = tblCard.CardID });
            }
            else
            {
                ModelState.AddModelError("", result.Message);
                return View(tblCard);
            }
        }

        private string GetOrSetCustomer(tblCardSubmit obj, HttpPostedFileBase FileUpload)
        {

          

            if (FileUpload != null)
            {
                var extension = Path.GetExtension(FileUpload.FileName) ?? "";
                var fileName = Path.GetFileName(string.Format("{0}{1}", StringUtil.RemoveSpecialCharactersVn(FileUpload.FileName.Replace(extension, "")).GetNormalizeString(), extension));
                var url = ConfigurationManager.AppSettings["FileUploadAvatar"];

                obj.CustomerAvatar = string.Format("{0}{1}", url, fileName);
            }
            var id = "";
            if (!string.IsNullOrWhiteSpace(obj.CustomerID))
            {
                var tblCus = _tblCustomerService.GetById(Guid.Parse(obj.CustomerID));
                if (tblCus != null)
                {
                    id = tblCus.CustomerID.ToString();
                    tblCus.CustomerCode = obj.CustomerCode;
                    tblCus.CustomerName = obj.CustomerName;
                    tblCus.CustomerGroupID = obj.CustomerGroupID;
                    tblCus.Address = obj.CustomerAddress;
                    tblCus.Avatar = obj.CustomerAvatar;
                    tblCus.IDNumber = obj.CustomerIdentify;
                    tblCus.Mobile = obj.CustomerMobile;
                    tblCus.CompartmentId = obj.CompartmentId;
                    _tblCustomerService.Update(tblCus);
                }
                else
                {
                     tblCus = new tblCustomer()
                    {
                        CustomerID = Guid.NewGuid(),
                        Address = obj.CustomerAddress,
                        Avatar = obj.CustomerAddress,
                        CustomerName = obj.CustomerName,
                        CustomerGroupID =obj.CustomerGroupID,
                        Description = "",
                        Mobile = obj.CustomerMobile,
                        IDNumber = obj.CustomerIdentify,
                        AccessLevelID = "",
                        CustomerCode = obj.CustomerCode,
                        Finger1 = "",
                        Finger2 = "",
                        Inactive = false,
                        SortOrder = 0,
                        DevPass ="",
                        CompartmentId = obj.CompartmentId,
                        AccessExpireDate = Convert.ToDateTime("2099/12/31")
                        
                    };
                    _tblCustomerService.Create(tblCus);
                }

            }
            else
            {
                if (!string.IsNullOrWhiteSpace(obj.CustomerCode))
                {
                    var tblCustomerByCode = _tblCustomerService.GetByCode(obj.CustomerCode);
                    if (tblCustomerByCode != null)
                    {
                        id = tblCustomerByCode.CustomerID.ToString();
                    }
                    else
                    {
                        var tbl = new tblCustomer()
                        {
                            CustomerID = Guid.NewGuid(),
                            Address = obj.CustomerAddress,
                            Avatar = obj.CustomerAvatar,
                            CustomerName = obj.CustomerName,
                            CustomerGroupID = obj.CustomerGroupID,
                            Description = "",
                            Mobile = obj.CustomerMobile,
                            IDNumber = obj.CustomerIdentify,
                            AccessLevelID = "",
                            CustomerCode = obj.CustomerCode,
                            Finger1 = "",
                            Finger2 = "",
                            Inactive = false,
                            SortOrder = 0,
                            DevPass = "",
                            CompartmentId = obj.CompartmentId,
                            AccessExpireDate = Convert.ToDateTime("2099/12/31")

                        };
                        var result = _tblCustomerService.Create(tbl);
                        if (result.isSuccess)
                        {
                            id = tbl.CustomerID.ToString();
                        }
                    }
                }
            }
            return id;
        }

        private IEnumerable<tblCardGroup> GetListCardGroup()
        {
            var query = _cardGroupService.GetAllActive();
            return query;
        }
        #endregion
        #region Cập nhật
        [HttpGet]
        public ActionResult Update(string id, string key, string cardgroups, string customergroups, string active, string fromdate, string todate, string isCheckByTime = "0", int page = 1)
        {
            ViewBag.lcustomergroups = GetMenuList();
            ViewBag.lcardgroups = GetListCardGroup();
            var obj = _itblCardService.GetCustomById(Guid.Parse(id));
            if (obj != null)
            {
                var t = _itblCardService.GetById(Guid.Parse(id));
                if (t != null)
                {
                    obj.DtpDateExpired = Convert.ToDateTime(t.ExpireDate).ToString("dd/MM/yyy");
                    obj.DtpDateRegisted = Convert.ToDateTime(t.DateRegister != null ? t.DateRegister : DateTime.Now).ToString("dd/MM/yyy");
                    obj.DtpDateReleased = DateTime.Now.ToString("dd/MM/yyyy");
                    obj.DtpDateActive = Convert.ToDateTime(t.DateActive != null ? t.DateActive : DateTime.Now).ToString("dd/MM/yyyy");
                }
            }
            return View(obj);
        }
        [HttpPost]
        public ActionResult Update(tblCardSubmit obj,HttpPostedFileBase FileUpload, string key, string cardgroups, string customergroups, string active, string fromdate, string todate, string isCheckByTime = "0", int page = 1)
        {
            var oldObj = _itblCardService.GetCustomById(Guid.Parse(obj.CardID));
            if (oldObj == null)
            {
                return View(obj);
            }
            //valid card
            if ((string.IsNullOrWhiteSpace(obj.CardNo) || string.IsNullOrEmpty(obj.CardNo)) || string.IsNullOrEmpty(obj.CardGroupID))
            {
                if ((string.IsNullOrWhiteSpace(obj.CardNo) || string.IsNullOrEmpty(obj.CardNo)))
                {
                    ModelState.AddModelError("CardNo", "Please enter CardNo");
                }
                if (string.IsNullOrEmpty(obj.CardGroupID))
                {
                    ModelState.AddModelError("CardGroupID", "Please enter CardGroupId");
                }
                return View(obj);
            }

            //if (FileUpload != null)
            //{
            //    var extension = Path.GetExtension(FileUpload.FileName);
            //    var fileName = Path.GetFileName(string.Format("{0}{1}", StringUtil.RemoveSpecialCharactersVn(FileUpload.FileName.Replace(extension, "")).GetNormalizeString(), extension));
            //    var url = ConfigurationManager.AppSettings["FileUploadAvatar"];

            //    oldObj.CustomerAvatar = string.Format("{0}{1}", url, fileName);
            //}   
            //Gán giá trị
            var result = new MessageReport();
            var map = _itblCardService.GetById(Guid.Parse(obj.CardID));
            if (map != null)
            {
                var existed = _itblCardService.GetByCardNumber_Id(map.CardNumber, Guid.Parse(obj.CardID));
                if (existed != null)
                {
                    ModelState.AddModelError("CardNumber", "mã thẻ đã tồn tại");
                    return View(obj);
                }
                map.CardNumber = obj.CardNumber;
            }
            //thẻ
            map.CardNo = obj.CardNo;
            map.CardGroupID = obj.CardGroupID;
            map.Description = !string.IsNullOrWhiteSpace(obj.CardDescription) ? obj.CardDescription : "";
            map.isAutoCapture = obj.IsAutoCapture;
            map.DVT = obj.DVT;
            //ngày
            map.DateRegister = Convert.ToDateTime(obj.DtpDateRegisted);
            map.DateRelease = Convert.ToDateTime(obj.DtpDateReleased);
            //khách hàng
            //obj.CustomerAvatar = oldObj.CustomerAvatar;
            map.CustomerID = GetOrSetCustomer(obj, FileUpload);
            result = _itblCardService.Update(map);
            if (result.isSuccess)
            {
                return RedirectToAction("Index", new { });
            }
            else
            {
                ModelState.AddModelError("", result.Message);
                return View(obj);
            }

        }
        #endregion
        #region Xóa
        public JsonResult Delete(string id)
        {
            var noti = FunctionHelper.GetLocalizeDictionary("Home", "notification");
            var obj = _itblCardService.GetById(Guid.Parse(id));
            if (obj == null)
            {
                var result1 = new MessageReport();
                result1.Message = noti["card_does_not_exist_in_the_system"];
                result1.isSuccess = false;
                return Json(result1, JsonRequestBehavior.AllowGet);
            }
            // check trong sự kiện
            var existedInEvent = _tblCardEvenService.GetAllByCardNumber(obj.CardNumber);
            if (existedInEvent.Any())
            {
                var result1 = new MessageReport();
                result1.Message = noti["card_is_existing_in_the_event"];
                result1.isSuccess = false;
                return Json(result1, JsonRequestBehavior.AllowGet);
            }

            var result = _itblCardService.DeleteById(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        // xóa không cần điều kiện
        public JsonResult Remove(string id)
        {
            var noti = FunctionHelper.GetLocalizeDictionary("Home", "notification");
            var obj = _itblCardService.GetById(Guid.Parse(id));
            if (obj == null)
            {
                var result1 = new MessageReport();
                result1.Message = noti["card_does_not_exist_in_the_system"];
                result1.isSuccess = false;
                return Json(result1, JsonRequestBehavior.AllowGet);
            }
            var result = _itblCardService.DeleteById(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        private List<SelectListModel2> GetListCardGroups()
        {

            var list = new List<SelectListModel2>();
            var query = _cardGroupService.GetAll().ToList();
            if (query.Any())
            {
                foreach (var item in query)
                {
                    list.Add(new SelectListModel2 { ItemValue = item.CardGroupID.ToString(), ItemOtherValue = item.CardType.ToString(), ItemText = item.CardGroupName });
                }
            }
            return list;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<SelectListModel> GetMenuList()
        {
            var list = new List<SelectListModel>();

            var MenuList = _tblCustomerGroupservice.GetAllActive();
            var parent = MenuList.Where(n => n.ParentID == "0" || n.ParentID == "");
            if (parent.Any())
            {
                foreach (var item in parent.OrderBy(n => n.SortOrder))
                {
                    //nếu có thì đuyệt để lưu vào list
                    list.Add(new SelectListModel { ItemValue = item.CustomerGroupID.ToString(), ItemText = item.CustomerGroupName });
                    //gọi action đê lấy danh sach con (submenu) theo id
                    var submenu = Children(item.CustomerGroupID.ToString());
                    // kiểm tra xem submenu có hay k

                    if (submenu.Count > 0)
                    {
                        // Nếu có thì duyêt tiếp để lưu và list
                        foreach (var item1 in submenu)
                        {
                            list.Add(new SelectListModel { ItemValue = item1.ItemValue.ToString(), ItemText = item.CustomerGroupName + "--" + item1.ItemText });

                        }
                        // Phân tách các danh mục
                        list.Add(new SelectListModel { ItemValue = "-1", ItemText = "--*--*--" });
                    }
                    else
                    {
                        // Phân tách các danh mục
                        list.Add(new SelectListModel { ItemValue = "-1", ItemText = "--*--*--" });
                    }
                }
            }
            return list;
        }

        private List<SelectListModel> Children(string parentID)
        {
            var list = new List<SelectListModel>();
            // Lấy danh sách submenu theo id truyền từ action parient theo id
            var menu = _tblCustomerGroupservice.GetAllChildByParentID(parentID).ToList();
            // kiêm tra xem submeunu có giá trị k
            if (menu.Any())
            {
                foreach (var item in menu)
                {
                    //Nếu có thì duyệt tiếp để lưu vào list
                    list.Add(new SelectListModel { ItemValue = item.CustomerGroupID.ToString(), ItemText = item.CustomerGroupName });
                    // //gọi action đê lấy danh sach con (submenu) theo id
                    var submenu = Children(item.CustomerGroupID.ToString());
                    if (submenu.Count > 0)
                    {
                        foreach (var item1 in submenu)
                        {
                            list.Add(new SelectListModel { ItemValue = item1.ItemValue.ToString(), ItemText = item.CustomerGroupName + "--" + item1.ItemText });
                        }
                    }
                }
            }
            return list;
        }

        private void GetListChildCustomer(List<string> str, string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {


                str.Add(id);
                var list = _tblCustomerGroupservice.GetAllChildActiveByParentID(id).ToList();
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        str.Add(item.CustomerGroupID.ToString());
                        GetListChildCustomer(str, item.CustomerGroupID.ToString());
                    }
                }

            }
        }
    }
}