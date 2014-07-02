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

        public UsersController(IUsersRepository usersRepo)
        {
            _usersRepo = usersRepo;
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
                return View(model);
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
                    HttpCookie cookie = FormsAuthentication.GetAuthCookie(model.Email.ToLowerInvariant(), false);
                    Response.Cookies.Add(cookie);
                    return Redirect("/");
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(model);
                }
            }
            else
            {
                return View(model);
            }
        }

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return Redirect("/");
        }
    }
}
