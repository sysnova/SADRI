using NHibernate.AspNet.Identity;
using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using SharpArch.NHibernate;
using System.Web.Mvc;


namespace SADRI.Web.Ui.ViewModels
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        /*
        public ApplicationUser()
            : base()
        {
            this.Groups = new HashSet<ApplicationUserGroup>();
        }

        [Required]
        public virtual string FirstName { get; set; }

        [Required]
        public virtual string LastName { get; set; }

        [Required]
        public virtual string Email { get; set; }

        public virtual ICollection<ApplicationUserGroup> Groups { get; set; }
        */
    }
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }

        public ApplicationRole(string name, string description)
            : base(name)
        {
            this.Description = description;
        }

        public virtual string Description { get; set; }
    }
//
    public class ApplicationRoleGroup
    {
        public virtual string RoleId { get; set; }
        public virtual int GroupId { get; set; }

        public virtual ApplicationRole Role { get; set; }
        public virtual Group Group { get; set; }
    }
    public class Group
    {
        public Group() { }


        public Group(string name)
            : this()
        {
            this.Roles = new List<ApplicationRoleGroup>();
            this.Name = name;
        }

        [Key]
        [Required]
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual ICollection<ApplicationRoleGroup> Roles { get; set; }
    }
    public class ApplicationUserGroup
    {
        [Required]
        public virtual string UserId { get; set; }
        [Required]
        public virtual int GroupId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Group Group { get; set; }
    }
// Fin Group Based Security

// Custom User Manager
public class ApplicationUserManager : UserManager<ApplicationUser>
{
    public ApplicationUserManager(IUserStore<ApplicationUser> _userStore)
        : base(_userStore)
  {
      this.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 4,
                RequireNonLetterOrDigit = false,
                RequireDigit = true,
                RequireLowercase = false,
                RequireUppercase = false,
            };
      this.UserLockoutEnabledByDefault = true;
      this.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
      this.MaxFailedAccessAttemptsBeforeLockout = 5;
   }
}
// Fin Custom User Manager
}