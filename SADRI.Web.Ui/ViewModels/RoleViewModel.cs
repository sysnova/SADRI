using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
// Modelo de Datos
using NHibernate.AspNet.Identity;

namespace SADRI.Web.Ui.ViewModels
{
    public class RoleViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
        //[Display(Name = "List Roles")]
        //public IEnumerable<ApplicationRole> ListRoles { get; set; }
        [Display(Description = "Description")]
        public string Description { get; set; }

    }
}