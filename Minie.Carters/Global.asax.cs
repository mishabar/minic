using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Minie.Carters.Repositories;
using MongoDB.Driver;

namespace Minie.Carters
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // initializing dependency injection  container
            var builder = new ContainerBuilder();

            // registrating all the existing controllers
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            MongoClient mongoClient = new MongoClient(ConfigurationManager.AppSettings["MongoDB"]);

            builder.Register(c => mongoClient.GetServer().GetDatabase(ConfigurationManager.AppSettings["MongoDB"].Split('/').Last())).AsSelf();
            builder.Register(c => new CategoriesRepository(c.Resolve<MongoDatabase>())).AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.Register(c => new ProductsRepository(c.Resolve<MongoDatabase>())).AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.Register(c => new OrdersRepository(c.Resolve<MongoDatabase>())).AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.Register(c => new UsersRepository(c.Resolve<MongoDatabase>())).AsImplementedInterfaces().InstancePerLifetimeScope();

            // build the dependencies
            IContainer container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        protected void Session_Start(Object sender, EventArgs e)
        {
            Session["init"] = true;
        }
    }
}