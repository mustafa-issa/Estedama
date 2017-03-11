using Highsoft.Web.Mvc.Charts;
using System;
using System.Collections.Generic;

namespace ChartsMix.Models
{
    public class PieGroupModel
    {
        public PieGroupModel()
        {
            Group = new Group();
        }
        public List<Series> Series { get; set; }
        public List<PieSeriesData> GroupData { get; set; }
        public PiePeriod period { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public Group Group { get; set; }
    }
}