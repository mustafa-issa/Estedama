using Highsoft.Web.Mvc.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChartsMix.Models
{
    [Serializable]
    public class CustomLineSeries : LineSeries
    {
         public List<LineSeriesData> things { get; set; }
    }
}