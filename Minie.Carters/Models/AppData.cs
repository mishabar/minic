using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Minie.Carters.Data;
using Minie.Carters.Interfaces.Repositories;

namespace Minie.Carters.Models
{
    public class AppData
    {
        public static IEnumerable<Category> Categories { get; set; }
        public static Dictionary<string, string> Orders = new Dictionary<string, string> { { "plh", "Price Low to High" }, { "phl", "Price High to Low" } };
        
        static AppData()
        {
            ICategoriesRepository categoriesRepo = (ICategoriesRepository)DependencyResolver.Current.GetService<ICategoriesRepository>();
            Categories = categoriesRepo.FindAll();
        }

        public static float ExchangeRate = 5.5F;
    }
}