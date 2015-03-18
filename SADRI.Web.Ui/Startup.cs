//
using Microsoft.Owin;
using Owin;
using SharpArch.NHibernate.Web.Mvc;
//
[assembly: OwinStartupAttribute(typeof(SADRI.Web.Ui.Startup))]
namespace SADRI.Web.Ui
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            ConfigureAuth(app);
            ConfigureData();
        }
        private static void ConfigureData()
        {
            var storage = new WebSessionStorage(System.Web.HttpContext.Current.ApplicationInstance);
            DataConfig.Configure(storage);
        }

    }
}
