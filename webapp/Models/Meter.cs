using System;
using System.Collections.Generic;

namespace ChartsMix.Models
{
    public class Meter
    {
        public Meter()
        {
            Children = new List<Meter>();
        }
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public int EntityId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }
        public virtual Meter Parent { get; set; }
        public virtual List<Meter> Children { get; set; }
    }
}