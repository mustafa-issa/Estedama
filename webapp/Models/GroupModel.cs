using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChartsMix.Models
{
    public class GroupModel
    {
        public PiePeriod period { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int[] Ids { get; set; }
    }
}