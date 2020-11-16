using Kztek.Model.Models;
using Kztek.Service.Admin;
using Kztek.Web.Core.Functions;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kztek.Web.Areas.Parking.Controllers
{
    public class tblCustomersController : Controller
    {
        #region service
        private ItblCustomerService itblCustomerService;
        private ItblCustomerGroupService itblCustomerGrService;
         public tblCustomersController (ItblCustomerService itblCustomerService , ItblCustomerGroupService itblCustomerGrService)
        {
            this.itblCustomerGrService = itblCustomerGrService;
            this.itblCustomerService = itblCustomerService;
        }
        #endregion

        #region DANH SÁCH CUSTOMER
        // GET: Parking/tblCustomers
        public ActionResult Index(string key = "", string customerGr ="", string selectedId = "", int page = 1, string chkExport = "0" )
        {
            int totalItem = 0;
            int pageSize = 20;
            var str = new List<string>();
            //GetCustomerGr(str, customerGr);
            var listCustomer = itblCustomerService.GetListCustmer(key, customerGr, selectedId, page, pageSize, ref totalItem);
            var girdModel = PageModelCustom<tblCustomer>.GetPage(listCustomer, page, pageSize, totalItem);
            if (listCustomer.Any())
            {
            var lstId = "";
                foreach (var item in listCustomer)
                {
                    lstId += item.CustomerGroupID + ",";


                }
            ViewBag.lstCustomerGr = itblCustomerGrService.GetAllActiveByListId(lstId).ToList();
            }
            return View(girdModel);
        }

        //private void GetCustomerGr(List<string> str, string customerGr)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion
    }
}