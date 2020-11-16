using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kztek.Model.Models
{
   public class TestDat
    {
        public string CompanyCode { get; set; }

      
        public DateTime TransactionDate { get; set; }

       
        public string DocumentNumber { get; set; }

        
        public string DocumentAmount { get; set; }

        public string ProjectCode { get; set; }

        public string TransactionCode { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime ParkingPeriod { get; set; }
        public string DebtorName { get; set; }
        public string UnitNumber { get; set; }
        public string CarParkNumber { get; set; }
        public string CarPlateNumber { get; set; }
        public string CarOwnerName { get; set; }
        public string TaxAmount { get; set; }
       
    }
}
