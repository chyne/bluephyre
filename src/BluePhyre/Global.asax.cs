using System.Web.Mvc;
using System.Web.Routing;
using BluePhyre.Infrastructure;
using Castle.Windsor;
using Castle.Windsor.Installer;

namespace BluePhyre
{

    public class MvcApplication : System.Web.HttpApplication
    {

        private static IWindsorContainer _container;

        private static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        private static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            BootstrapContainer();
        }

        protected void Application_End()
        {

            _container.Dispose();
        }

        private static void BootstrapContainer()
        {
            _container = new WindsorContainer()
            .Install(FromAssembly.This());
            var controllerFactory = new WindsorControllerFactory(_container.Kernel);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }

    }
}