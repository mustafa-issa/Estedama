using Highsoft.Web.Mvc.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChartsMix.Models
{
    public class LineChartModel
    {
        public LineChartModel()
        {
            TreeRoot = new Meter();
        }
        public Meter TreeRoot { get; set; }
        public BarPeriod period { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int[] Ids { get; set; }
    }
}