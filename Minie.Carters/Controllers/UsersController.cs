using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Minie.Carters.Data;
using Minie.Carters.Interfaces.Repositories;
using Minie.Carters.Models;
using Minie.Carters.Repositories;

namespace Minie.Carters.Controllers
{
    public class UsersController : Controller
    {
        private IUsersRepository _usersRepo = null;
        private IOrdersRepository _ordersRepo = null;

        public UsersController(IUsersRepository usersRepo, IOrdersRepository ordersRepo)
        {
            _usersRepo = usersRepo;
            _ordersRepo = ordersRepo;
        }

        [HttpGet]
        public ActionResult SignIn()
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView();
            }

            return Redirect("/");
        }

        [HttpPost]
        public ActionResult SignIn(UserSignin model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_usersRepo.ValidateUser(model.Email.ToLowerInvariant(), model.Password))
                    {
                        HttpCookie cookie = FormsAuthentication.GetAuthCookie(model.Email.ToLowerInvariant(), false);
                        Response.Cookies.Add(cookie);
                        Order order = _ordersRepo.GetCurrentCart(Session.SessionID, null);
                        if (order != null)
                        {
                            order.UserId = model.Email;
                            _ordersRepo.Save(order);
                        }

                        return Json(new { status = "OK" });
                    }
                    return Json(new { error = "Email inválido ou senha incorreta" });
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(model);
                }
            }
            else
            {
                if (Request.IsAjaxRequest())
                {
                    return Json(new { error = string.Join("<br/>", ModelState.Values.SelectMany(x => x.Errors.Select(y => y.ErrorMessage))) }); 
                }
                else
                {
                    return View(model);
                }
            }
        }

        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(UserSignup model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check unique username and email
                    if (!_usersRepo.IsUniqueEmail(model.Email))
                        throw new InvalidOperationException("Email já está em uso");

                    _usersRepo.Save(new User { Email = model.Email.ToLowerInvariant(), Name = model.Name, Password = PasswordHash.CreateHash(model.Password) });
                    Order order = _ordersRepo.GetCurrentCart(Session.SessionID, null);
                    if (order != null)
                    {
                        order.UserId = model.Email;
                        _ordersRepo.Save(order);
                    }
                    HttpCookie cookie = FormsAuthentication.GetAuthCookie(model.Email.ToLowerInvariant(), false);
                    Response.Cookies.Add(cookie);

                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { status = "OK" });
                    }
                    else
                    {
                        return Redirect("/");
                    }
                }
                catch (InvalidOperationException ex)
                {
                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { error = string.Join("<br/>", ModelState.Values.SelectMany(x => x.Errors.Select(y => y.ErrorMessage))) });
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                        return View(model);
                    }
                }
            }
            else
            {
                if (Request.IsAjaxRequest())
                {
                    return Json(new { error = string.Join("<br/>", ModelState.Values.SelectMany(x => x.Errors.Select(y => y.ErrorMessage))) });
                }
                else
                {
                    return View(model);
                }
            }
        }

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
            return Redirect("/");
        }
    }
}
