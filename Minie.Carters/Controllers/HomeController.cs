using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Minie.Carters.Data;
using Minie.Carters.Interfaces.Repositories;
using Minie.Carters.Models;

namespace Minie.Carters.Controllers
{
    public class HomeController : Controller
    {
        private IProductsRepository _productsRepo = null;
        private ICategoriesRepository _categoriesRepo = null;

        public HomeController(IProductsRepository productsRepo, ICategoriesRepository categoriesRepo)
        {
            _productsRepo = productsRepo;
            _categoriesRepo = categoriesRepo;
        }

        public ActionResult Index()
        {
            List<HomeOffer> model = new List<HomeOffer>();
            foreach (var category in AppData.Categories)
            {
                var clearance = category.SubCategories.FirstOrDefault(c => c.CategoryID.ToLower().Contains("clearance"));
                if (clearance != null)
                {
                    HomeOffer offer = new HomeOffer { CategoryName = category.Name, CategoryID = clearance.CategoryID };
                    int pages = 0;
                    IEnumerable<Product> products = _productsRepo.GetByCategory(clearance.CategoryID, new string[0], "plh", 1, out pages);
                    if (products.Any())
                    {
                        Random rnd = new Random(DateTime.Now.Millisecond);
                        offer.Products = products.OrderBy(p => rnd.Next()).Take(4).ToArray();
                        model.Add(offer);
                    }
                }
            }
            return View(model);
        }

        public ActionResult SizeChart()
        {
            return PartialView("_Sizes");
        }
    }
}
