using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minie.Carters.Data
{
    public class OrderItem
    {
        public string SKU { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
    }
}
