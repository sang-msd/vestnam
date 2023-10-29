using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Data.EF;
using Data.Services;
using TGClothes.Models;

namespace TGClothes.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IRateService _rateService;
        private readonly ISizeService _sizeService;
        private readonly IUserService _userService;

        public ProductController(
            IProductService productService, 
            IProductCategoryService productCategoryService, 
            IRateService rateService,
            ISizeService sizeService,
            IUserService userService)
        {
            _productService = productService;
            _productCategoryService = productCategoryService;
            _rateService = rateService;
            _sizeService = sizeService;
            _userService = userService;
        }

        // GET: Admin/Product
        public ActionResult Index()
        {
            var model = _productService.GetAll();
            return View(model);
        }


        [ChildActionOnly]
        public PartialViewResult ProductCategory()
        {
            var model = _productCategoryService.GetAll();
            return PartialView(model);
        }

        public ActionResult Category(long productCategoryId)
        {
            var category = _productCategoryService.GetProductCategoryById(productCategoryId);
            return View(category);
        }

        public ActionResult Detail(long id)
        {
            var product = _productService.GetProductById(id);
            ViewBag.Category = _productCategoryService.GetProductCategoryById(product.CategoryId.Value);
            ViewBag.RelateProducts = _productService.ListRelateProduct(id);
            ViewBag.Sizes = _sizeService.GetAll();
            return View(product);
        }
    }
}
