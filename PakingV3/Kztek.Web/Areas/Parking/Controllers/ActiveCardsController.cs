using Kztek.Model.CustomModel;
using Kztek.Model.Models;
using Kztek.Service.Admin;
using Kztek.Web.Core.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kztek.Web.Areas.Parking.Controllers
{
    public class ActiveCardsController : Controller
    {

        #region DI
        private ItblCardService _tblCardService;
        private ItblCardGroupService _tblCardGroupService;
        private ItblCustomerGroupService _tblCustomerGr;
        public ActiveCardsController (tblCardGroupService _tblCardGroupService, ItblCustomerGroupService _tblCustomerGr, ItblCardService _tblCardService)
        {
            this._tblCardGroupService = _tblCardGroupService;
            this._tblCustomerGr = _tblCustomerGr;
            this._tblCardService = _tblCardService;
        }
        #endregion
        // GET: Parking/ActiveCards
        public ActionResult Index()
        {
            ViewBag.CardGroup = GetCardGroup().Where(n => n.CardType == 0);
            ViewBag.CustomerGr = GetMenulist();

            return View();
        }

        public PartialViewResult boxCards(string key="",string fromdate="",string todate="",string cargroup="",string customerGr="", int page =1)
        {
            var str = "";
           var customergr= GetCustomerGr(str, customerGr);
            int pageSize = 20;
            int total = 0;
            var list = _tblCardService.GetAllPagingByFirsts_Sql(key, fromdate, todate, customergr,cargroup,ref total, page, pageSize);
           
            var girdMode = PageModelCustom<tblCardExtend>.GetPage(list, pageSize, page, total);
            return PartialView(girdMode);
        }









        #region Ds hỗ trợ
        private object GetCustomerGr(string str, string customerGr)
        {
            throw new NotImplementedException();
        }
        private List<SelectListModel> GetMenulist()

        {
            var list = new List<SelectListModel>();
            var menuList = _tblCustomerGr.GetAllActive().ToList() ;
            var parent = menuList.Where(n => n.ParentID == "0" || n.ParentID == "");
            if (parent.Any())
            {
                foreach (var item in parent.OrderBy(c => c.SortOrder))
                {
                    //Nếu có thì duyệt tiếp để lưu vào list
                    list.Add(new SelectListModel { ItemValue = item.CustomerGroupID.ToString(), ItemText = item.CustomerGroupName });


                    var submenu = Children(item.CustomerGroupID.ToString());
                    if (submenu.Count > 0)
                    {
                        foreach (var item1 in submenu)

                        {
                            list.Add(new SelectListModel
                            {
                                ItemValue =item1.ItemValue.ToString(), ItemText= item.CustomerGroupID.ToString() + "--" +  item1.ItemText
                            });
                            list.Add(new SelectListModel
                            {
                                ItemValue = "-1",
                                ItemText = "-----------"
                            });

                        }
                    }
               
                }

            }

            return list;

        }

        private List<SelectListModel> Children(string parentId)
        {
            var lst = new List<SelectListModel>();
            var children = _tblCustomerGr.GetAllChildByParentID(parentId.ToString()).ToList();
            if (children.Any())
            {
                foreach (var item in children)
                {
                    lst.Add(new SelectListModel
                    {
                       ItemValue=item.CustomerGroupID.ToString() ,ItemText =item.CustomerGroupName
                    });
                    var submenu = Children(item.CustomerGroupID.ToString());
                    if (submenu.Count >0)
                    {
                        foreach (var item1 in submenu)
                        {
                            lst.Add(new SelectListModel
                            {
                                ItemValue = item1.ItemValue,
                                ItemText = item.CustomerGroupID.ToString() + "--" +
                                item1.ItemText
                            });
                        }
                    }

                }
            }
            return lst;
        }

        private IEnumerable<tblCardGroup> GetCardGroup()
        {
            

            return _tblCardGroupService.GetAllActive();
        }
        #endregion
    }
}