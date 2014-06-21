using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Minie.Carters.Models
{
    public class SetItemQuantityModel
    {
        public string SKU { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
    }
}