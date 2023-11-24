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
using TGClothes.Common;
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
        private readonly IOrderService _orderService;
        private readonly IOrderDetailService _orderDetailService;

        public ProductController(
            IProductService productService, 
            IProductCategoryService productCategoryService, 
            IRateService rateService,
            ISizeService sizeService,
            IProductSizeService productSizeService,
            IUserService userService,
            IGalleryService galleryService, 
            IOrderService orderService,
            IOrderDetailService orderDetailService)
        {
            _productService = productService;
            _productCategoryService = productCategoryService;
            _rateService = rateService;
            _sizeService = sizeService;
            _productSizeService = productSizeService;
            _userService = userService;
            _galleryService = galleryService;
            _orderService = orderService;
            _orderDetailService = orderDetailService;
        }

        // GET: Admin/Product
        public ActionResult Index()
        {
            var model = _productService.GetAll();
            return View(model);
        }

        public ActionResult ProductByCategory(long id, int page = 1, int pageSize = 4)
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
                                 where ps.Stock > 0
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
            ViewBag.Review = from r in _rateService.GetAll()
                             join u in _userService.GetAll() on r.UserId equals u.Id
                             where (r.ProductId == id && r.UserId == u.Id)
                             select new UserRateModel
                             {
                                 Rate = r,
                                 User = u
                             };

            var data = from o in _orderService.GetAll()
                                        join od in _orderDetailService.GetAll() on o.Id equals od.OrderId
                                        join u in _userService.GetAll() on o.CustomerId equals u.Id
                                        where (od.ProductId == id && o.CustomerId == u.Id)
                                        select new CustomerPerchasedModel
                                        {
                                            User = u,
                                            Order = o,
                                            OrderDetail = od
                                        };
            ViewBag.CustomerPurchased = data.Count();
            Session["ProductId"] = product.Id;
            ViewBag.CountRate = CountRate(id);
            ViewBag.CountRateFiveStar = CountRateFiveStar(id);
            if (ViewBag.CountRate == 0)
            {
                ViewBag.AverageRate = null;
            }
            else
            {
                var rate = _rateService.GetRateByProductId(id);
                var rateSum = rate.Sum(x => x.Star) ?? 0;
                var countRate = rate.Count();
                var avgRate = (float)rateSum / countRate;

                ViewBag.AverageRate = avgRate;
            }
            return View(product);
        }

        [HttpPost]
        public ActionResult ProductReviews(Rate review, int? rating, string content)
        {
            var user = (UserLogin)Session[CommonConstants.USER_SESSION];
            review.CreatedDate = DateTime.Now;
            review.Content = content;
            review.UserId = _userService.GetUserByEmail(user.Email).Id;
            review.Star = rating;
            review.ProductId = (long)Session["ProductId"];

            _rateService.Insert(review);
            return RedirectToAction("Detail", "Product", new { id = review.ProductId });
        }

        [ChildActionOnly]
        public ActionResult ModalDetail(long id)
        {
            var product = _productService.GetProductById(id);
            var size = _productSizeService.GetProductSizeByProductId(product.Id);
            ViewBag.Category = _productCategoryService.GetProductCategoryById(product.CategoryId.Value);
            ViewBag.GetAllCategory = _productCategoryService.GetAll();
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
            return PartialView("~/Views/Product/ModalDetail.cshtml", product);
        }

        public int CountRateFiveStar(long id)
        {
            List<Product> product = _productService.GetAll();
            List<Rate> rate = _rateService.GetAll();
            int countRate = (from r in rate
                             join p in product
                             on r.ProductId equals p.Id
                             where (r.ProductId == id && r.ProductId == p.Id && r.Star == 5)
                             select new ProductRateModel
                             {
                                 Rate = r,
                                 Product = p
                             }).Count();
            return countRate;
        }

        public int CountRate(long id)
        {
            List<Product> product = _productService.GetAll();
            List<Rate> rate = _rateService.GetAll();
            int countRate = (from r in rate
                             join p in product
                             on r.ProductId equals p.Id
                             where (r.ProductId == id && r.ProductId == p.Id && r.Star != null)
                             select new ProductRateModel
                             {
                                 Rate = r,
                                 Product = p
                             }).Count();
            return countRate;
        }

        public JsonResult ListName(string term)
        {
        var data = _productService.ListName(term);
            return Json(new
            {
                data = data,
                status = true
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Search(string searchkeyword, int page = 1, int pageSize = 2)
        {
            int totalRecord = 0;

            int maxPage = 5;
            int totalPage = 0;

            var result = _productService.Search(searchkeyword, ref totalRecord, page, pageSize);

            ViewBag.TotalRecord = totalRecord;
            ViewBag.Page = page;
            ViewBag.Keyword = searchkeyword;
            totalPage = (int)Math.Ceiling((double)totalRecord / pageSize);
            ViewBag.TotalPage = totalPage;
            ViewBag.MaxPage = maxPage;
            ViewBag.First = 1;
            ViewBag.Last = totalPage;
            ViewBag.Next = page + 1;
            ViewBag.Prev = page - 1;
            return View(result);
        }
    }
}
