using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mercadopago;
using Minie.Carters.Data;
using Minie.Carters.Interfaces.Repositories;
using Minie.Carters.Models;
using Newtonsoft.Json;

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
        public ActionResult DoCheckout()
        {
            string sessionId = Session.SessionID;
            string userId = null;

            // get user
            if (User.Identity.IsAuthenticated)
            {
                userId = _usersRepo.Find(User.Identity.Name).Email;
            }
            Order order = _ordersRepo.GetCurrentCart(sessionId, userId).AdjustItemPrices();
            if (order == null || order.Items.Count == 0)
            {
                return Redirect("/");
            }

            MP mp = new MP(ConfigurationManager.AppSettings["MPClientID"], ConfigurationManager.AppSettings["MPSecret"]);
            mp.sandboxMode(bool.Parse(ConfigurationManager.AppSettings["MPSandbox"]));
            var data = new
            {
                items = order.Items.Select(i => new { title = i.Name, quantity = i.Quantity, currency_id = "BRL", unit_price = i.Price }).ToArray(),
                back_urls = new
                {
                    success = "http://" + Request.Url.DnsSafeHost + Url.RouteUrl("CheckoutStatus"),
                    failure = "http://" + Request.Url.DnsSafeHost + Url.RouteUrl("CheckoutStatus"),
                    pending = "http://" + Request.Url.DnsSafeHost + Url.RouteUrl("CheckoutStatus")
                }
            };
            Hashtable preference = mp.createPreference(JsonConvert.SerializeObject(data));

            order = _ordersRepo.GetCurrentCart(sessionId, userId);
            order.MPRefID = (string)((Hashtable)preference["response"])["id"];

            _ordersRepo.Save(order);

            return Json(new { url = (string)((Hashtable)preference["response"])[ConfigurationManager.AppSettings["MPUrl"]] });
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

        [HttpGet]
        public ActionResult CheckoutStatus()
        {
            string mpRefID = Request["preference_id"];
            string status = Request["collection_status"];
            string collectionID = Request["collection_id"];

            if (string.IsNullOrWhiteSpace(mpRefID) || string.IsNullOrWhiteSpace(status) || string.IsNullOrWhiteSpace(collectionID))
            {
                return Redirect("/");
            }
            else
            {
                Order order = _ordersRepo.GetByMPRefID(mpRefID);
                order.MPCollectionID = collectionID;
                order.Status = status;
                _ordersRepo.Save(order);

                //NotifyUserOrderStatus();

                return View("Status", order.AdjustItemPrices());
            }
        }

        [HttpGet]
        public ActionResult Notification()
        {
            string topic = Request["topic"];
            string id = Request["id"];

            MP mp = new MP(ConfigurationManager.AppSettings["MPClientID"], ConfigurationManager.AppSettings["MPSecret"]);
            mp.sandboxMode(bool.Parse(ConfigurationManager.AppSettings["MPSandbox"]));

            Hashtable paymentInfo = mp.getPaymentInfo(id);

            //NotifyUserOrderStatus();
            //NotifyBuyerOrderStatus();

            Order order = _ordersRepo.GetByMPCollectionID(id);
            order.Status = ((Hashtable)((Hashtable)paymentInfo["response"])["collection"])["status"].ToString();
            _ordersRepo.Save(order);

            return Json(new { status = "OK" }, JsonRequestBehavior.AllowGet);
        }
    }
}
