using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChartsMix.Models
{
    public class TableValues
    {
        public string Date { get; set; }
        public double Value { get; set; }
        public double PreviousValue { get; set; }
        public double Consumption { get; set; }
    }
}