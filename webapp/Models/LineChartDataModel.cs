using Highsoft.Web.Mvc.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChartsMix.Models
{
    public class LineChartDataModel
    {
        public virtual List<CustomLineSeries> Result { get; set; }
        public ChartDetails Details { get; set; }
        public double MaxConsumption { get; set; }
        public double MinConsumption { get; set; }
        public double AverageCunsumption { get; set; }
    }
}