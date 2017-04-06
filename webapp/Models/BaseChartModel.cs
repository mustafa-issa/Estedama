using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChartsMix.Models
{
    public class BaseChartModel<T>
    {
        public BaseChartModel()
        {
            MaxConsumption = double.MinValue;
            MinConsumption = double.MaxValue;
        }
        public double MaxConsumption { get; set; }
        public double MinConsumption { get; set; }
        public double AverageConsumption { get; set; }
        public T Data { get; set; }
    }
}