﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kztek.Model.CustomModel.iParking
{
    public class tblCameraCustomViewModel
    {
        public string CameraID { get; set; }
  

        public string CameraName { get; set; }

        public string PCID { get; set; }

        public string PCName { get; set; }

        public string HttpUrl { get; set; }

        public bool Inactive { get; set; }

        public int SortOrder { get; set; }
    }
}
