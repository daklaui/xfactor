using System.Web.Optimization;
using WebHelpers.Mvc5;

namespace xfactor.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            "~/Scripts/jquery.validate*"));

            bundles.Add(new StyleBundle("~/Bundles/css")
                .Include("~/Content/css/bootstrap.min.css", new CssRewriteUrlTransformAbsolute())
                .Include("~/Content/css/bootstrap-select.css")
                .Include("~/Content/css/bootstrap-datepicker3.min.css")
                .Include("~/Content/css/font-awesome.min.css")
                .Include("~/Content/css/icheck/blue.min.css", new CssRewriteUrlTransformAbsolute())
                .Include("~/Content/css/AdminLTE.css", new CssRewriteUrlTransformAbsolute())
                .Include("~/Content/css/skins/skin-blue.css")
                .Include("~/Content/css/ionicons.min.css")
                .Include("~/Content/css/select2.min.css")
                .Include("~/Content/css/dataTables.bootstrap.min.css")
                .Include("~/Content/css/buttons.dataTables.min.css")
                //.Include("~/Content/css/jquery.dataTables.min.css")
                //.Include("~/Content/css/select.dataTables.min.css")
              );

            bundles.Add(new ScriptBundle("~/Bundles/js")
                .Include("~/Content/js/plugins/jquery/jquery-3.3.1.js")
                  .Include("~/Content/js/plugins/bootstrap/bootstrap.js")
                .Include("~/Content/js/plugins/fastclick/fastclick.js")
                .Include("~/Content/js/plugins/slimscroll/jquery.slimscroll.js")
                .Include("~/Content/js/plugins/bootstrap-select/bootstrap-select.js")
                .Include("~/Content/js/plugins/datepicker/bootstrap-datepicker.js")
                .Include("~/Content/js/plugins/icheck/icheck.js")
                .Include("~/Content/js/plugins/select2.full.min.js")
                .Include("~/Content/js/plugins/inputmask/jquery.inputmask.bundle.js")
                .Include("~/Content/js/plugins/jquery.dataTables.min.js")
                .Include("~/Content/js/plugins/dataTables.bootstrap.min.js")
                .Include("~/Content/js/dataTables.buttons.min.js")
                 .Include("~/Content/js/buttons.flash.min.js")
                 .Include("~/Content/js/jszip.min.js")
                .Include("~/Content/js/pdfmake.min.js")
                .Include("~/Content/js/vfs_fonts.js")
                 .Include("~/Content/js/buttons.html5.min.js")
                  .Include("~/Content/js/buttons.print.min.js")
                  .Include("~/Content/js/dataTables.select.min.js")
                .Include("~/Content/js/adminlte.js")
                .Include("~/Content/js/init.js"));


        }
    }
}
