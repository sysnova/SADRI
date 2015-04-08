using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web;
using SADRI.Services.Interfaces;

namespace SADRI.Web.Ui.Filters
{
    public class CheckPermissionAttribute : AuthorizeAttribute //SiteMapSecurityt Trimming implementa AttributeAuthorize o su Custom
    {
        private string _role;
        public CheckPermissionAttribute(string RolCustom)
        {
            _role = RolCustom;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {

            bool authorized = false;
            //IProductService _productService = (IProductService)DependencyResolver.Current.GetService(typeof(IProductService));

            if ((HttpContext.Current.User.IsInRole(_role)) && (HttpContext.Current.User.Identity.IsAuthenticated))
            {
                //if (HttpContext.Current.User.Identity.Name == "lgonzalez13U") { return false; }
                authorized = true;
            }

            return authorized;

        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {

            if (this.AuthorizeCore(filterContext.HttpContext))
            {
                base.OnAuthorization(filterContext);
                // Completar para agregar lógica extra en el proceso de autorizacion
            }
            else
            {
                this.HandleUnauthorizedRequest(filterContext);
            }
            
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
                filterContext.Result = new HttpStatusCodeResult(403);
            else
                filterContext.Result = new HttpUnauthorizedResult();
        } 

     }
}
