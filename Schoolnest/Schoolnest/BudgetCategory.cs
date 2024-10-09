using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Schoolnest
{
    public class BudgetCategory
    {
        public string CategoryName { get; set; }
        public decimal Amount { get; set; }
        public decimal ActualExpenditure { get; set; }
        public decimal UtilizationPercentage { get; set; }  // New property
    }
}