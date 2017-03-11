using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChartsMix.Models
{
    public class Group
    {
        public Group()
        {
            TreeRoot = new Meter();
        }
        public Meter TreeRoot { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int[] Ids { get; set; }

    }
}