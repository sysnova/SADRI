using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SADRI.Domain.Entities;

namespace SADRI.Services.Interfaces
{
    public interface IProductService
    {
        IEnumerable<Category> GetCategories();
        IEnumerable<Product> GetProducts();
        IEnumerable<Product> GetProductByCategoryId(int idCategory);
    }
}
