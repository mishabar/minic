using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Minie.Carters.Data;

namespace Minie.Carters.Models
{
    public class HomeOffer
    {
        public string CategoryName { get; set; }
        public string CategoryID { get; set; }
        public Product[] Products { get; set; }
    }
}