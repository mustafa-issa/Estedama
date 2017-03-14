using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChartsMix.Models
{
    public class GroupDataModel
    {
        public GroupDataModel()
        {
            subGroups = new List<GroupDataModel>();
        }
        public string name { get; set; }
        public double y { get; set; }
        public List<GroupDataModel> subGroups { get; set; }
        public string drilldown { get; set; }
        public string id { get; set; }
    }
}