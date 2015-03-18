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
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private RoleStore<IdentityRole> _roleStore; //{ get; set; }

        public RoleController(RoleStore<IdentityRole> roleStore)
        {
            _roleStore = roleStore;
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
                var role = new IdentityRole(model.Name);   
                await _roleStore.CreateAsync(role);
                return RedirectToAction("AddRole", "Role");
            }
            return View(model);
        }
        
        private RoleViewModel GetRolesViewModel()
        {
            IEnumerable<IdentityRole> roles = Enumerable.Empty<IdentityRole>();
            roles = _roleStore.Roles;
            RoleViewModel roleViewModel = new RoleViewModel
            {
                Name = "",
                ListRoles = roles
            };
            return roleViewModel;
        }
    }
}