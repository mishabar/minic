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
    public class OrdersController : Controller
    {
        private IProductsRepository _productsRepo = null;
        private IOrdersRepository _ordersRepo = null;
        private IUsersRepository _usersRepo = null;

        public OrdersController(IProductsRepository productsRepo, IOrdersRepository ordersRepo, IUsersRepository usersRepo)
        {
            _productsRepo = productsRepo;
            _ordersRepo = ordersRepo;
            _usersRepo = usersRepo;
        }

        [HttpPost]
        public ActionResult AddItem(AddItemModel model)
        {
            string sessionId = Session.SessionID;
            string userId = null;

            try
            {
                // get user
                if (User.Identity.IsAuthenticated)
                {
                    userId = _usersRepo.Find(User.Identity.Name).Email;
                }

                // get product
                Product product = _productsRepo.Get(model.SKU);

                // ensure size available
                if (product.Sizes.Contains(model.Size))
                {
                    _ordersRepo.AddItem(sessionId, userId, product.ToOrderItem(model.Size));
                }

                return Json(new { cart = _ordersRepo.GetCurrentCart(sessionId, userId).AdjustItemPrices() });
            }
            catch
            {
                return Json(new { error = "Unexpected error occurred. Please try again.", cart = _ordersRepo.GetCurrentCart(sessionId, userId).AdjustItemPrices() });
            }
        }

        [HttpPost]
        public ActionResult RemoveItem(RemoveItemModel model)
        {
            string sessionId = Session.SessionID;
            string userId = null;

            try
            {
                // get user
                if (User.Identity.IsAuthenticated)
                {
                    userId = _usersRepo.Find(User.Identity.Name).Email;
                }

                _ordersRepo.RemoveItem(sessionId, userId, model.SKU, model.Size);

                return Json(new { cart = _ordersRepo.GetCurrentCart(sessionId, userId).AdjustItemPrices() });
            }
            catch
            {
                return Json(new { error = "Unexpected error occurred. Please try again.", cart = _ordersRepo.GetCurrentCart(sessionId, userId).AdjustItemPrices() });
            }
        }

        [HttpGet]
        public ActionResult MyCart()
        {
            string sessionId = Session.SessionID;
            string userId = null;

            try
            {
                // get user
                if (User.Identity.IsAuthenticated)
                {
                    userId = _usersRepo.Find(User.Identity.Name).Email;
                }

                return Json(new { cart = _ordersRepo.GetCurrentCart(sessionId, userId).AdjustItemPrices() }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { error = "Unexpected error occurred. Please try again.", cart = _ordersRepo.GetCurrentCart(sessionId, userId).AdjustItemPrices() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Checkout()
        {
            string sessionId = Session.SessionID;
            string userId = null;

            // get user
            if (User.Identity.IsAuthenticated)
            {
                userId = _usersRepo.Find(User.Identity.Name).Email;
            }

            return View(_ordersRepo.GetCurrentCart(sessionId, userId).AdjustItemPrices());
        }

        [HttpPost]
        public ActionResult SetItemQuantity(SetItemQuantityModel model)
        {
            string sessionId = Session.SessionID;
            string userId = null;

            try
            {
                // get user
                if (User.Identity.IsAuthenticated)
                {
                    userId = _usersRepo.Find(User.Identity.Name).Email;
                }

                // get product
                Product product = _productsRepo.Get(model.SKU);

                // ensure size available
                if (product.Sizes.Contains(model.Size))
                {
                    _ordersRepo.SetItemQuantity(sessionId, userId, model.SKU, model.Size, model.Quantity);
                }

                return Json(new { cart = _ordersRepo.GetCurrentCart(sessionId, userId).AdjustItemPrices() });
            }
            catch
            {
                return Json(new { error = "Unexpected error occurred. Please try again.", cart = _ordersRepo.GetCurrentCart(sessionId, userId).AdjustItemPrices() });
            }
        }
    }
}
