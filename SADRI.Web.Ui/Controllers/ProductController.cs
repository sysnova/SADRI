using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using SADRI.Domain.Entities;
using SADRI.Web.Ui.ViewModels;
using SADRI.Services.Interfaces;
using SADRI.Infrastructure.Interfaces;
using SADRI.Web.Ui.Filters;

namespace SADRI.Web.Ui.Controllers
{
    [CheckPermission("User")]
    public class ProductController : Controller
    {
        // GET: Product
        // Services will be injected
        private IProductService _productService;
        private ILoggingService _loggingService;

        public ProductController(IProductService productService, ILoggingService loggingService)
        {
            _productService = productService;
            _loggingService = loggingService;
        }

        public ActionResult Index()
        {
            // Get view model
            ProductViewModel viewModel = GetProductViewModel(0);

            // Log action
            _loggingService.Trace("GET Action: ProductController.Index");

            // Return view
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Index(int categoryId)
        {
            // Get view model
            ProductViewModel viewModel = GetProductViewModel(categoryId);

            // Log action
            _loggingService.Trace("POST Action: ProductController.Index");

            // Return view
            return View(viewModel);
        }

        public ProductViewModel GetProductViewModel(int selectedCategoryId)
        {
            // Get categories
            var categories = _productService.GetCategories();
            // Get products
            IEnumerable<Product> products = Enumerable.Empty<Product>();

            if (selectedCategoryId == 0)
            {
                products = _productService.GetProducts();
            }

            else
            {
                products = _productService.GetProductByCategoryId(selectedCategoryId);
            }


            // Set selected category
            Category selectedCategory = null;

            /*
            int categoryId = selectedCategoryId.GetValueOrDefault();
            if (categories.Count() > 0)
            {
                if (categoryId > 0)
                {
                    selectedCategory = (from c in categories
                                        where c.CategoryId == categoryId
                                        select c).FirstOrDefault();
                }
            }
            */

            // Return product view model
            ProductViewModel productViewModel = new ProductViewModel
            {
                Categories = categories,
                SelectedCategory = selectedCategory,
                Products = products
            };
            return productViewModel;
        }

    }
}