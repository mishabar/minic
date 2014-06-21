using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Minie.Carters.Models
{
    public class ItemsIndex<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int Pages { get; set; }
        public string Category { get; set; }
        public int Page { get; set; }
        public string[] Sizes { get; set; }
        public string Order { get; set; }
    }
}