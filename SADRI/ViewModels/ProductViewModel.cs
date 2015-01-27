using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using SADRI.Domain.Entities;

namespace SADRI.Web.Ui.ViewModels
{
    public class ProductViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
        public Category SelectedCategory { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}