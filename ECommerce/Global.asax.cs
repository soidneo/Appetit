using ECommerce.Clases;
using ECommerce.Migrations;
using ECommerce.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ECommerce
{
    public class MvcApplication : HttpApplication
    {
        string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        protected void Application_Start()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ECommerceContext, Migrations.Configuration>());
            CheckRolesAndSuperUser();
            InstallHelper.InitialConfiguration();
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            SqlDependency.Start(connString);
        }

        protected void Application_End()
        {
            //Stop SQL dependency
            SqlDependency.Stop(connString);
        }

        private void CheckRolesAndSuperUser()
        {
            UsuariosHelper.CheckRole("SuperAdmin");
            UsuariosHelper.CheckRole("Admin");
            UsuariosHelper.CheckRole("Seller");
            UsuariosHelper.CheckRole("Provider");
            UsuariosHelper.CheckRole("Customer");
            UsuariosHelper.CheckRole("Cash");
            UsuariosHelper.CheckRole("Waiter");
            UsuariosHelper.CheckRole("VirtualWaiter");
            UsuariosHelper.CheckRole("Kitchen");
            UsuariosHelper.CheckSuperUser();
        }
    }
}
