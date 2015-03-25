using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using SADRI.Domain.Entities;
using SADRI.Domain.Interfaces;
using SADRI.Services.Interfaces;

namespace SADRI.Services.ImplServices
{
    public class ProductService : IProductService
    {
        // Repositories will be injected
        IRepository<Category> _categoriesRep;
        IRepository<Product> _productsRep;
        IProductRepository _implProductsRep;

        public ProductService(IRepository<Category> categoriesRep,
            IRepository<Product> productsRep, IProductRepository implProductsRep)
        {
            _categoriesRep = categoriesRep;
            _productsRep = productsRep;
            _implProductsRep = implProductsRep;
        }

        public IEnumerable<Category> GetCategories()
        {
            // Return all categories
            IEnumerable<Category> categories = _categoriesRep.GetAll();
            return categories;
        }

        public IEnumerable<Product> GetProducts()
        {

            IEnumerable<Product> products = _productsRep.GetAll();

            return products;
        }
        public IEnumerable<Product> GetProductByCategoryId(int idCategory)
        {
            IEnumerable<Product> products = _implProductsRep.GetProductByCategoryId(idCategory);

            return products;
            //Return object

        }
    }
}
