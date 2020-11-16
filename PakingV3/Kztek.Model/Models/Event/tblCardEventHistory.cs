﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kztek.Model.Models.Event
{
    public class tblCardEventHistory
    {
        [Key]
        public System.Guid Id { get; set; }
        public string EventCode { get; set; }
        public string CardNumber { get; set; }
        public Nullable<System.DateTime> DatetimeIn { get; set; }
        public Nullable<System.DateTime> DateTimeOut { get; set; }
        public string PicDirIn { get; set; }
        public string PicDirOut { get; set; }
        public string LaneIDIn { get; set; }
        public string LaneIDOut { get; set; }
        public string UserIDIn { get; set; }
        public string UserIDOut { get; set; }
        public string PlateIn { get; set; }
        public string PlateOut { get; set; }
        public string RegistedPlate { get; set; }
        public decimal Moneys { get; set; }
        public string CardGroupID { get; set; }
        public string VehicleGroupID { get; set; }
        public string CustomerGroupID { get; set; }
        public string CustomerName { get; set; }
        public bool IsBlackList { get; set; }
        public bool IsFree { get; set; }
        public bool IsDelete { get; set; }
        public string FreeType { get; set; }
        public string CardNo { get; set; }
    }
}
