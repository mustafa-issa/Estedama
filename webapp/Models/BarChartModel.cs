using Highsoft.Web.Mvc.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChartsMix.Models
{
    public class BarChartModel
    {
        public BarChartModel()
        {
            TreeRoot = new Meter();
        }
        public Meter TreeRoot { get; set; }
        public List<Series> Data { get; set; }
        public List<string> Dates { get; set; }
        public BarPeriod period { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int[] Ids { get; set; }
        public double StandardValue { get; set; }
    }
}