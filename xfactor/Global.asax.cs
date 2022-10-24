using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using xfactor.App_Start;
using xfactor.Controllers;

namespace xfactor
{
    public class MvcApplication : HttpApplication
    {
       // data source = ; initial catalog = ; user id = ; password=xpertiseit2019
      string con = @"Data Source=192.168.49.151;Initial Catalog=Xfactor_prod_web;Persist Security Info=True;User ID=medfactor;Password=xfactor2013;MultipleActiveResultSets=True";
        //   string con = @"Data Source=192.168.49.151;Initial Catalog=XFactor_WEB_VTEST;Persist Security Info=True;User ID=medfactor;Password=xfactor2013;MultipleActiveResultSets=True";
      // string con = @"Data Source=51.210.243.165;Initial Catalog=Xfactor_R;Persist Security Info=True;User ID=xpertiseit;Password=Xpertiseit2019;MultipleActiveResultSets=True";
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            SqlDependency.Start(con);
        }
       
       
        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            var httpContext = ((HttpApplication)sender).Context;
            httpContext.Response.Clear();
            httpContext.ClearError();

            if (new HttpRequestWrapper(httpContext.Request).IsAjaxRequest())
            {
                return;
            }

            //ExecuteErrorController(httpContext, exception as HttpException);
        }

        //private void ExecuteErrorController(HttpContext httpContext, HttpException exception)
        //{
        //    var routeData = new RouteData();
        //    routeData.Values["controller"] = "Error";

        //    if (exception != null && exception.GetHttpCode() == (int)HttpStatusCode.NotFound)
        //    {
        //        routeData.Values["action"] = "NotFound";
        //    }
        //    else
        //    {
        //        routeData.Values["action"] = "InternalServerError";
        //    }

        //    using (Controller controller = new ErrorController())
        //    {
        //        ((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
        //    }
        //}
    }
}