using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChartsMix.Models
{
    public class ChartDetails
    {
        public ChartDetails()
        {
            Dates = new List<string>();
        }
        public List<string> Dates { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
    }
}