using Kztek.Model.CustomModel;
using Kztek.Model.Models;
using Kztek.Service;
using Kztek.Service.Admin;
using Kztek.Web.Core.Extensions;
using Kztek.Web.Core.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kztek.Web.Areas.Parking.Controllers
{
    public class ReportNewController : Controller
    {

        #region
        private ItblCustomerGroupService _tblCustomerGroupservice;
        private IReportService _ReportService;
        private ItblLaneService _LaneService;
        private ItblCardGroupService _CardGroupService;
        private IUserService _UserService;
        private ItblVehicleGroupService _VehicleService;
        public ReportNewController
          (IReportService _ReportService, ItblCustomerGroupService _tblCustomerGroupservice, ItblLaneService _LaneService, ItblCardGroupService _CardGroupService , IUserService _UserService, ItblVehicleGroupService _VehicleService)
        {
            this._ReportService = _ReportService;
            this._tblCustomerGroupservice = _tblCustomerGroupservice;
            this._LaneService = _LaneService;
            this._CardGroupService = _CardGroupService;
            this._UserService = _UserService;
            this._VehicleService = _VehicleService;
        }
        #endregion

        #region Xe trong bãi tại thời điểm bất kì
        public ActionResult ReportVehicleAnyTimes (string fromdate = "",int page =1 , string datefrompicker = "")
        {
            ////var pageSize = 20;
            ////var totalItem = 0;
            ////var list = _ReportService.GetReportVehicleAnyTimes(fromdate, page, pageSize, ref totalItem);
            ////var girldModel = PageModelCustom<ReportVehicleInAnyTime>.GetPage(list, page, pageSize,totalItem);
            ////ViewBag.DateFromPickerValue = !string.IsNullOrWhiteSpace(fromdate) ? Convert.ToDateTime(fromdate).ToString("dd/MM/yyyy HH:mm:59") : DateTime.Now.ToString("dd/MM/yyyy HH:mm:59");

            ////return View(girldModel);
            //var pageSize = 20;

            //var Dictionary = FunctionHelper.GetLocalizeDictionary("report", "reportVehicleInAnyTime");
            


            //var list = _ReportService.GetReportVehicleAnyTimes(fromdate, page, pageSize);

            //ViewBag.DateFromPickerValue = !string.IsNullOrWhiteSpace(fromdate) ? Convert.ToDateTime(fromdate).ToString("dd/MM/yyyy HH:mm:59") : DateTime.Now.ToString("dd/MM/yyyy HH:mm:59");

            //var table = Data.Event.SqlHelper.ExcuteSQLEvent.ConvertTo<ReportVehicleInAnyTime>(list);

            //var totalItem = 0;
            //var list = _ReportService.GetReportVehicleAnyTimes(fromdate, page, pageSize, ref totalItem);
            //var girldModel = PageModelCustom<ReportVehicleInAnyTime>.GetPage(list, page, pageSize,totalItem);
            //ViewBag.DateFromPickerValue = !string.IsNullOrWhiteSpace(fromdate) ? Convert.ToDateTime(fromdate).ToString("dd/MM/yyyy HH:mm:59") : DateTime.Now.ToString("dd/MM/yyyy HH:mm:59");


            return null;
        }
        #endregion


        #region Chi tiết xe trong bãi ở tại thời điểm bất kỳ
        public ActionResult ReportDetailVehicleAnyTimes(string key = "", string fromdate = "", string vehiclegroupid = "", string customegroup = "", string cardgroup = "", int page = 1)
        {
            var pageSize = 20;
            var totalItem = 0;
            var str =new  List<string>();
            GetListChild(str, customegroup);

            var datefrompicker = "";
            if (!string.IsNullOrWhiteSpace(fromdate))
            {   
                datefrompicker = fromdate;
            }
            var lstVehicleGroupID = new List<string>();
            if (!string.IsNullOrEmpty(vehiclegroupid))
            {
                var t = vehiclegroupid.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (t.Any())
                {
                    foreach (var item in t)
                    {
                        lstVehicleGroupID.Add(item);
                    }
                }
            }
            var list = _ReportService.GetReportDetailVehicleAnyTimse(key, fromdate, lstVehicleGroupID, str, cardgroup, page, pageSize, ref totalItem);
           
            var gridModel = PageModelCustom<ReportIn>.GetPage(list, page, pageSize, totalItem);
            ViewBag.Lanes = GetLaneList().ToList();
            ViewBag.Users = GetAllUser().ToList();
            ViewBag.KeyWord = key;
            ViewBag.fromDate =!string.IsNullOrEmpty(fromdate) ? Convert.ToDateTime(fromdate).ToString("dd/MM/yyy HH:mm:59") : DateTime.Now.ToString("dd/MM/yyyy HH:mm:59");

            ViewBag.Cardgroups = GetListCardGroups().ToList();
            ViewBag.CardGroupDT = GetListCardGroups().ToDataTableNullable();
            ViewBag.CardGroupId = cardgroup;

            ViewBag.VehicleGroupDT = GetListVehicle().ToDataTableNullable();
            ViewBag.VehicleID = vehiclegroupid;
            ViewBag.CustomerGroup = GetMenuList();
            ViewBag.CustomGroupId = customegroup;
            return View (gridModel);
        }

        private IEnumerable<tblVehicleGroup> GetListVehicle()
        {
            return _VehicleService.GetAll();
        }



        #endregion


        #region xe trong bãi hiện tại
        // GET: Parking/ReportNew
        public ActionResult ReportIns(string key = "", bool IsFilterByTimeIn = false, string groupCustomer = "", string fromdate = "", string todate = "", string lane = "", string user = "", string cardgroups = "", int page = 1)
        {
            int pageSize = 20;
            int totalItem = 0;


            // tạo list chưa list customergroup child
            var str = new List<string>();
           GetListChild(str, groupCustomer);

            // tìm kiếm theo date 
            var datefrompicker = "";
            if (string.IsNullOrWhiteSpace(fromdate))
            {
                fromdate = DateTime.Now.ToString("dd/MM/yyyy 00:00:00");

            }
            if (string.IsNullOrWhiteSpace(todate))
            {
                todate = DateTime.Now.ToString("dd/MM/yyyy 23:59:59");
            }


            if (!string.IsNullOrWhiteSpace(fromdate) && !string.IsNullOrWhiteSpace(todate))
            {
                datefrompicker = fromdate + "-" + todate;
            }



            var list = _ReportService.GetReportIns(key, IsFilterByTimeIn, str,user,  fromdate, todate, cardgroups, lane, page, pageSize, ref totalItem);
            if (list.Any())
            {

                //tốc độ lấy dữ liệu nhanh hơn
                var strs = "";
                foreach (var item in list)
                {
                    strs = item.LaneIDIn + "," + item.LaneIDOut + ",";
                }

                ViewBag.listLane = _LaneService.GetAllActiveByListIds(strs).ToList();
            }

            var gridModel = PageModelCustom<ReportIn>.GetPage(list, page, pageSize, totalItem);
            // truyền dữ liệu từ controller sang view
            ViewBag.keyword = key;
           

            // 
            ViewBag.CardGroup = GetListCardGroups().ToList();
            // tìm kiếm theo nhóm thẻ
            ViewBag.CardGroupDT = GetListCardGroups().ToDataTableNullable();
            ViewBag.CardsGroups = cardgroups;

            // V
            ViewBag.GroupCustomers = GetMenuList();
            ViewBag.GroupCustomerID = groupCustomer;


            //truyền dữ liệu lane sang view
            ViewBag.Lanes = GetLaneList().ToList();
            ViewBag.LaneDT = GetLaneList().ToDataTableNullable();
            ViewBag.LaneId = lane;

            ViewBag.fromdateValue = fromdate;
            ViewBag.todateValue = !string.IsNullOrWhiteSpace(todate) ? Convert.ToDateTime(todate).ToString("dd/MM/yyyy HH:mm:59") : DateTime.Now.ToString("dd/MM/yyyy 23:59:59");
            ViewBag.CheckTime = IsFilterByTimeIn;

            return View(gridModel);
        }
        #endregion
        /// <summary>
        /// Dánh sach menu cấp cha
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
        /// <summary>
        /// Đệ quy lấy danh sách con
        /// </summary>
        /// <param name="parentID"></param>
        /// <returns></returns>
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
                    if (submenu .Count > 0)
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

        private List<SelectListModel2> GetListCardGroups()
        {
            var list = new List<SelectListModel2>();
            var cardgroups = _CardGroupService.GetAll().ToList();
            if (cardgroups.Any())
            {
                foreach (var item in cardgroups)
                {
                    list.Add(new SelectListModel2 { ItemValue = item.CardGroupID.ToString(), ItemOtherValue = item.CardType.ToString(), ItemText = item.CardGroupName });
                }

            }
            if (_ReportService.SystemUsingLoop())
            {
                list.Add(new SelectListModel2 { ItemValue = "LOOP_D", ItemOtherValue = "loop", ItemText = "Vòng từ - Xe lượt(Loop)" });
                list.Add(new SelectListModel2 { ItemValue = "LOOP_M", ItemOtherValue = "loop", ItemText = "Vòng từ-Xe tháng(Loop)" });
            }
            return list;
        }

        private IEnumerable<tblLane> GetLaneList()
        {

            var query = _LaneService.GetAll();
            return query;
        }

       // Lấy list customer group child
        private void GetListChild(List<string> strCG, string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {

                strCG.Add(id);

                // lấy tất cả parentID của customergroup với điều kiệu Actice = true và parent =id
                var list = _tblCustomerGroupservice.GetAllChildActiveByParentID(id).ToList();
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        //phân câp hệ thống theo kiểu cha con
                        strCG.Add(item.CustomerGroupID.ToString());
                        // đệ quy ; để phân cấp theo cấp cha con
                        GetListChild(strCG, item.CustomerGroupID.ToString());
                    }
                }
            }

        }

        #region
        // Xe ra ngoài bãi

        public ActionResult ReportOut ( string key = "", bool ischeckTime = false, string groupCustomer="",string lane ="", string user ="",string cardgroup ="",string fromdate="",string todate ="",int page = 1)
            {
            // tìm kiếm theo key
            //     var checkKey = !string.IsNullOrWhiteSpace(key) ? key.Replace(".", "").Replace(",", "").Replace("  ", "") : String.Empty;
            //tim kiems theo nhóm khách hang
            var strs = new List<string>();
            GetListChild(strs, groupCustomer);
            // tìm kiếm theo ngay tháng
            var findDate = "";

            if (string.IsNullOrWhiteSpace(fromdate))
            {
                fromdate = DateTime.Now.ToString("dd/MM/yyyy 00:00:00");
            }
            if (string.IsNullOrWhiteSpace(todate))
            {
                todate = DateTime.Now.ToString("dd/MM/yyyy 00:00:00");
            }
            if (!string.IsNullOrWhiteSpace(fromdate) && !string.IsNullOrWhiteSpace(todate))
            {
                findDate = fromdate + "-" + todate;
            }
            
            var pagesize = 20;
            int totalItem = 0;
            var list = _ReportService.GetReportOuts(key, ischeckTime, fromdate,todate, strs, lane, user, cardgroup, page, pagesize, ref totalItem);
           
            if (list.Any())
            {
                var str = "";
                foreach (var item in list)
                {
                    str = item.LaneIDIn + "," + item.LaneIDOut + ","; 
                }
                ViewBag.ListLane = _LaneService.GetAllActiveByListIds(str).ToList();
            }
            
            var girdModel = PageModelCustom<ReportInOut>.GetPage(list, page, pagesize, totalItem);

            // truyền list card group sang view
            ViewBag.CardGroups = GetCardGroups().ToList();
            ViewBag.CarGroupDT = GetCardGroups().ToDataTableNullable();
            ViewBag.CardGroupId = cardgroup;

            //ViewBag.ListLanes = GetLaneList().ToList();
            ViewBag.LaneDT = GetLaneList().ToDataTableNullable();
            ViewBag.LaneID = lane;

            ViewBag.CustomerGr = GetMenuList();
            ViewBag.CustomerGrID = groupCustomer;
            // 
            ViewBag.User = GetAllUser().ToList();

            // truyen viewBag key để tìm kiếm
            ViewBag.KeyWord = key;
            ViewBag.CheckTime = ischeckTime;
            // tìm kiếm theo date
            ViewBag.Fromdate = fromdate;
            ViewBag.ToDate = !string.IsNullOrWhiteSpace(todate) ? Convert.ToDateTime(todate).ToString("dd/MM/yyyy HH:mm:59") :
                DateTime.Now.ToString("dd/MM/yyyy 00:00:00");
            return View(girdModel);
        }

        //private List<SelectListModel> GetMenuLists()
        //{
        //    var lists = new List<SelectListModel>();
        //    var menuList = _tblCustomerGroupservice.GetAllActive();
        //    var parent = menuList.Where(n => n.ParentID == "0" || n.ParentID == "");
        //    if (parent.Any())
        //    {
        //        foreach (var item in parent.OrderBy( n => n.SortOrder ))
        //        {
        //            //nếu có thì đuyệt để lưu vào list
        //            lists.Add(new SelectListModel { ItemText = item.CustomerGroupID.ToString(), ItemValue = item.CustomerGroupName });
        //            //gọi action đê lấy danh sach con (submenu) theo id
        //            var submenu = Childrens(item.CustomerGroupID.ToString());
        //            if (submenu.Count > 0)
        //            {
        //                foreach (var item1 in submenu)
        //                {
        //                    lists.Add(new SelectListModel { ItemValue = item1.ItemValue, ItemText = item.CustomerGroupName + "--" + item1.ItemText });
        //                }
        //                // phan tach các danh mục
                        
        //            }
        //            else
        //            {
        //                // phan tach các danh mục
        //                lists.Add(new SelectListModel {ItemText= "-1", ItemValue="--**--" });
        //            }
        //        }
        //    }

        //    return lists;
        //}

        //private List<SelectListModel> Childrens(string parentId)
        //{
        //    //var list = new List<SelectListModel>();
        //    //var menu = _tblCustomerGroupservice.GetAllChildByParentID(parentId);
        //    //// kiêm tra xem submeunu có giá trị k
        //    //if (menu.Any())
        //    //{
        //    //    foreach (var item in menu)
        //    //    {
        //    //        list.Add(new SelectListModel { ItemText = item.CustomerGroupID.ToString(), ItemValue = item.CustomerGroupName });
        //    //        var submenu = Childrens(item.CustomerGroupID.ToString());
        //    //        // gọi action đê lấy danh sach con(submenu) theo id
        //    //        if (submenu.Count > 0)
        //    //        {
        //    //            foreach (var item1 in submenu)
        //    //            {
        //    //                list.Add(new SelectListModel { ItemValue = item1.ItemValue.ToString(), ItemText = item.CustomerGroupName + "--" + item1.ItemText });
        //    //            }
        //    //        }

        //    //    }
        //    var list = new List<SelectListModel>();
        //    // Lấy danh sách submenu theo id truyền từ action parient theo id
        //    var menu = _tblCustomerGroupservice.GetAllChildByParentID(parentId).ToList();
        //    // kiêm tra xem submeunu có giá trị k
        //    if (menu.Any())
        //    {
        //        foreach (var item in menu)
        //        {
        //            //Nếu có thì duyệt tiếp để lưu vào list
        //            list.Add(new SelectListModel { ItemValue = item.CustomerGroupID.ToString(), ItemText = item.CustomerGroupName });
        //            // //gọi action đê lấy danh sach con (submenu) theo id
        //            var submenu = Children(item.CustomerGroupID.ToString());
        //            if (submenu.Count > 0)
        //            {
        //                foreach (var item1 in submenu)
        //                {
        //                    list.Add(new SelectListModel { ItemValue = item1.ItemValue.ToString(), ItemText = item.CustomerGroupName + "--" + item1.ItemText });
        //                }
        //            }
        //        }
        //    }
        //    return list;

        //}

        //private void GetChildCustomerGroup(List<string> strs, string id)
        //{
        //    if (!string.IsNullOrWhiteSpace(id))
        //    {
        //        strs.Add(id);
        //        // lấy ds theo parentId của customer
        //        var list = _tblCustomerGroupservice.GetAllChildActiveByParentID(id).ToList();
                
        //        foreach (var item in list)
        //        {
        //            strs.Add(item.CustomerGroupID.ToString());
        //            GetChildCustomerGroup(strs, item.CustomerGroupID.ToString());
        //        }
        //    }
           
        //}

        private List<SelectListModel2> GetCardGroups()
        {
            var list = new List<SelectListModel2>();
            var cardgroups = _CardGroupService.GetAll().ToList();
            if (cardgroups .Any())
            {

                foreach (var item in cardgroups)
                {
                    list.Add(new SelectListModel2 { ItemValue = item.CardGroupID.ToString(), ItemOtherValue = item.CardType.ToString(), ItemText = item.CardGroupName });

                }
            }
            //if (_ReportService.SystemUsingLoop())
            //{
            //    list.Add(new SelectListModel2 { ItemValue = "LOOP_D", ItemOtherValue = "loop", ItemText = "Vòng từ - Xe lượt(Loop)" });
            //    list.Add(new SelectListModel2 { ItemValue = "LOOP_M", ItemOtherValue = "loop", ItemText = "Vòng từ-Xe tháng(Loop)" });
            //}
            return list;
        }

        private IEnumerable<User> GetAllUser()
        {
            return _UserService.GetAll();
        }

        #endregion
    }


}
