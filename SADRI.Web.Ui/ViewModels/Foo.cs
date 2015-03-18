using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpArch.Domain.DomainModel;


namespace SADRI.Web.Ui.ViewModels
{
    public class Foo : Entity
    {
        public ApplicationUser User { get; set; }
    }
}