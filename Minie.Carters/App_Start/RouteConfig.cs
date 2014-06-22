using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Minie.Carters
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "UserSignin",
                url: "signin",
                defaults: new { controller = "Users", action = "SignIn" }
            );

            routes.MapRoute(
                name: "UserSignup",
                url: "signup",
                defaults: new { controller = "Users", action = "SignUp" }
            );

            routes.MapRoute(
                name: "SignOut",
                url: "signout",
                defaults: new { controller = "Users", action = "SignOut" }
            );

            routes.MapRoute(
                name: "AddItemToOrder",
                url: "Orders/AddItem",
                defaults: new { controller = "Orders", action = "AddItem" }
                );

            routes.MapRoute(
                name: "RemoveItemFromOrder",
                url: "Orders/RemoveItem",
                defaults: new { controller = "Orders", action = "RemoveItem" }
                );

            routes.MapRoute(
                name: "SetItemQuantity",
                url: "Orders/SetItemQuantity",
                defaults: new { controller = "Orders", action = "SetItemQuantity" }
                );

            routes.MapRoute(
                name: "MyCart",
                url: "Orders/MyCart",
                defaults: new { controller = "Orders", action = "MyCart" }
                );

            routes.MapRoute(
                name: "Checkout",
                url: "Orders/Checkout",
                defaults: new { controller = "Orders", action = "Checkout" }
                );

            routes.MapRoute(
                name: "DoCheckout",
                url: "Orders/DoCheckout",
                defaults: new { controller = "Orders", action = "DoCheckout" }
                );

            routes.MapRoute(
                name: "CheckoutStatus",
                url: "Orders/CheckoutStatus",
                defaults: new { controller = "Orders", action = "CheckoutStatus" }
                );

            routes.MapRoute(
                name: "PaymentNotification",
                url: "Orders/Notification",
                defaults: new { controller = "Orders", action = "Notification" }
                );

            routes.MapRoute(
                name: "ProductsIndexWithSizes",
                url: "{category}/{page}",
                defaults: new { controller = "Products", action = "Index", sizes = "", page = 1 });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}