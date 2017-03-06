using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChartsMix.Models
{
    public class LineChartItem
    {
        public string Name { get; set; }
        public int entityId { get; set; }
        public DateTime date { get; set; }
        public double value { get; set; }
    }
}