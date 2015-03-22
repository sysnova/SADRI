using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using NHibernate.AspNet.Identity;
using SADRI.Web.Ui.ViewModels;
//

namespace SADRI.Web.Ui.Controllers
{
    [Authorize (Roles = "Admin")]
    public class RoleController : Controller
    {
        private RoleManager<ApplicationRole> _roleManager; //{ get; set; }

        public RoleController(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public ActionResult AddRole()
        {
            RoleViewModel model = GetRolesViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> AddRole(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = new ApplicationRole(model.Name, "Custom Rol");   
                await _roleManager.CreateAsync(role);
                return RedirectToAction("AddRole", "Role");
            }
            return View(model);
        }
        
        private RoleViewModel GetRolesViewModel()
        {
            IEnumerable<ApplicationRole> roles = Enumerable.Empty<ApplicationRole>();
            roles = _roleManager.Roles;
            RoleViewModel roleViewModel = new RoleViewModel
            {
                Name = "",
                ListRoles = roles
            };
            return roleViewModel;
        }
    }
}