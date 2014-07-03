using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Minie.Carters.Data;
using Minie.Carters.Interfaces.Repositories;
using Minie.Carters.Models;
using MongoDB.Driver.Builders;

namespace Minie.Carters.Controllers
{
    public class ProductsController : Controller
    {
        private IProductsRepository _productsRepo = null;
        private ICategoriesRepository _categoriesRepo = null;

        public ProductsController(IProductsRepository productsRepo, ICategoriesRepository categoriesRepo)
        {
            _productsRepo = productsRepo;
            _categoriesRepo = categoriesRepo;
        }

        //
        // GET: /Products/
        public ActionResult Index(string category, int page)
        {
            int pages = 1;
            string[] sizes = (string.IsNullOrWhiteSpace(Request["sizes"]) ? new string[0] : Request["sizes"].Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries));
            string order = (string.IsNullOrWhiteSpace(Request["order"]) ? "plh" : Request["order"]);
            Category cat = AppData.Categories.FirstOrDefault(c => c.CategoryID == category || c.SubCategories.Any(sc => sc.CategoryID == category));
            if (cat == null)
            {
                return Redirect("/");
            }
            else if (!cat.SubCategories.Any(sc => sc.CategoryID == category))
            {
                category = cat.SubCategories[0].CategoryID;
            }
            IEnumerable<Product> products = _productsRepo.GetByCategory(category, sizes, order, page, out pages);
            return View(new ItemsIndex<Product> { Items = products, Pages = pages, Page = page, Category = category, Sizes = sizes, Order = order });
        }

        public ActionResult Details(string sku)
        {
            Product product = _productsRepo.Get(sku);
            System.Diagnostics.Trace.WriteLine("|" + Request.UserAgent + "|", "DEBUG");
            if (Request.UserAgent == "facebookexternalhit/1.1 (+http://www.facebook.com/externalhit_uatext.php)")
            {
                return View("FBProduct", product);
            }
            else
            {
                return PartialView("_Product", product);
            }
        }
    }
}
