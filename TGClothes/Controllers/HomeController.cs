using Data.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TGClothes.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly IFooterService _footerService;
        private readonly ISlideService _slideService;
        private readonly IProductService _productService;

        public HomeController(IMenuService menuService, IFooterService footerService, ISlideService slideService, IProductService productService)
        {
            _menuService = menuService;
            _footerService = footerService;
            _slideService = slideService;
            _productService = productService;
        }

        // GET: Home
        public ActionResult Index()
        {
            ViewBag.Slides = _slideService.GetAll();
            ViewBag.NewProducts = _productService.ListNewProduct(5);
            ViewBag.FeatureProducts = _productService.ListFeatureProduct(5);
            return View();
        }

        [ChildActionOnly]
        public ActionResult MainMenu()
        {
            var model = _menuService.GetByGroupId(1);
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult Footer()
        {
            var model = _footerService.GetFooter();
            return PartialView(model);
        }
    }
}