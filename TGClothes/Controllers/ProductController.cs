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
        private readonly IProductSizeService _productSizeService;
        private readonly IUserService _userService;
        private readonly IGalleryService _galleryService;

        public ProductController(
            IProductService productService, 
            IProductCategoryService productCategoryService, 
            IRateService rateService,
            ISizeService sizeService,
            IProductSizeService productSizeService,
            IUserService userService,
            IGalleryService galleryService)
        {
            _productService = productService;
            _productCategoryService = productCategoryService;
            _rateService = rateService;
            _sizeService = sizeService;
            _productSizeService = productSizeService;
            _userService = userService;
            _galleryService = galleryService;
        }

        // GET: Admin/Product
        public ActionResult Index()
        {
            var model = _productService.GetAll();
            return View(model);
        }

        public ActionResult ProductByCategory(long id, int page = 1, int pageSize = 2)
        {
            var data = _productCategoryService.GetProductCategoryById(id);
            int totalRecord = 0;

            int maxPage = 5;
            int totalPage = 0;

            if (data.ParentId == null)
            {
                var result = _productService.GetAll(ref totalRecord, page, pageSize);

                ViewBag.Category = data;
                ViewBag.TotalRecord = totalRecord;
                ViewBag.Page = page;

                totalPage = (int)Math.Ceiling((double)totalRecord / pageSize);
                ViewBag.TotalPage = totalPage;
                ViewBag.MaxPage = maxPage;
                ViewBag.First = 1;
                ViewBag.Last = totalPage;
                ViewBag.Next = page + 1;
                ViewBag.Prev = page - 1;
                return View(result);
            }
            var model = _productService.GetProductByCategoryId(id, ref totalRecord, page, pageSize);

            ViewBag.Category = data;
            ViewBag.TotalRecord = totalRecord;
            ViewBag.Page = page;

            totalPage = (int)Math.Ceiling((double)totalRecord / pageSize);
            ViewBag.TotalPage = totalPage;
            ViewBag.MaxPage = maxPage;
            ViewBag.First = 1;
            ViewBag.Last = totalPage;
            ViewBag.Next = page + 1;
            ViewBag.Prev = page - 1;

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
            var size = _productSizeService.GetProductSizeByProductId(product.Id);
            ViewBag.Category = _productCategoryService.GetProductCategoryById(product.CategoryId.Value);
            ViewBag.GetAllCategory = _productCategoryService.GetAll();
            ViewBag.RelateProducts = _productService.ListRelateProduct(id);
            ViewBag.Sizes = _sizeService.GetAll();
            ViewBag.SizeStock = (from ps in _productSizeService.GetProductSizeByProductId(product.Id)
                                 join s in _sizeService.GetAll() on ps.SizeId equals s.Id
                                 select new ProductDetailModel
                                 {
                                     ProductId = ps.ProductId,
                                     SizeId = ps.SizeId,
                                     SizeName = s.Name,
                                     Stock = ps.Stock
                                 }).ToList();
            ViewBag.Galleries = (from p in _productService.GetAll().Where(p => p.Id == id)
                                join g in _galleryService.GetAll() on p.GalleryId equals g.Id
                                select new ProductGalleryModel
                                {
                                    Product = p,
                                    Gallery = g
                                }).FirstOrDefault();
            return View(product);
        }

        [ChildActionOnly]
        public ActionResult ModalDetail(long? id)
        {
            var product = _productService.GetProductById(id.Value);
            var size = _productSizeService.GetProductSizeByProductId(product.Id);
            ViewBag.Category = _productCategoryService.GetProductCategoryById(product.CategoryId.Value);
            ViewBag.GetAllCategory = _productCategoryService.GetAll();
            ViewBag.RelateProducts = _productService.ListRelateProduct(id.Value);
            ViewBag.Sizes = _sizeService.GetAll();
            ViewBag.SizeStock = (from ps in _productSizeService.GetProductSizeByProductId(product.Id)
                                 join s in _sizeService.GetAll() on ps.SizeId equals s.Id
                                 select new ProductDetailModel
                                 {
                                     ProductId = ps.ProductId,
                                     SizeId = ps.SizeId,
                                     SizeName = s.Name,
                                     Stock = ps.Stock
                                 }).ToList();
            ViewBag.Galleries = (from p in _productService.GetAll().Where(p => p.Id == id)
                                 join g in _galleryService.GetAll() on p.GalleryId equals g.Id
                                 select new ProductGalleryModel
                                 {
                                     Product = p,
                                     Gallery = g
                                 }).FirstOrDefault();
            return View(product);
        }
    }
}
