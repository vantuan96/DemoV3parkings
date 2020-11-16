﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kztek.Model.Models
{
    public class tblAccessControllerGroup
    {
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int SortOrder { get; set; }
    }
}
