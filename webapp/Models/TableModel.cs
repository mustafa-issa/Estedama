using System.Collections.Generic;
using Highsoft.Web.Mvc.Charts;
using System.Web.Mvc;
using System;
using System.Data;

namespace ChartsMix.Models
{
    public class TableModel
    {
        public TableModel()
        {
            TreeRoot = new Meter();
        }
        public Meter TreeRoot { get; set; }
        public PiePeriod period { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int Id { get; set; }
        public List<TableValues> values { get; set; }
    }
}