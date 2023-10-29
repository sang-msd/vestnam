using Data.EF;
using Data.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using TGClothes.Models;

namespace TGClothes.Areas.Admin.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IRateService _rateService;
        private readonly IUserService _userService;
        private readonly IGalleryService _galleryService;
        private readonly IProductSizeService _productSizeService;
        private readonly ISizeService _sizeService;

        public ProductController(
            IProductService productService, 
            IProductCategoryService productCategoryService, 
            IRateService rateService, 
            IUserService userService,
            IGalleryService galleryService,
            IProductSizeService productSizeService,
            ISizeService sizeService
            )
        {
            _productService = productService;
            _productCategoryService = productCategoryService;
            _rateService = rateService;
            _userService = userService;
            _galleryService = galleryService;
            _productSizeService = productSizeService;
            _sizeService = sizeService;
        }

        // GET: Admin/Product
        public ActionResult Index(string searchString, int page = 1, int pageSize = 10)
        {
            var model = _productService.GetAllPaging(searchString, page, pageSize);

            ViewBag.SearchString = searchString;
            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            SetViewBag();
            return View();
        }

        [HttpPost]
        public ActionResult Create(ProductImageModel model)
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Name = model.Product.Name,
                    MetaTitle = model.Product.MetaTitle,
                    Description = model.Product.Description,
                    Image = model.Product.Image,
                    Price = model.Product.Price,
                    Promotion = model.Product.Promotion,
                    PromotionPrice = model.Product.PromotionPrice,
                    CategoryId = model.Product.CategoryId,
                    CreatedDate = DateTime.Now,
                    MetaKeywords = model.Product.MetaKeywords,
                    MetaDescription = model.Product.MetaDescription,
                    Status = true,
                    TopHot = null,
                    ViewCount = 0
                };

                long id = _productService.Insert(product);
                if (id > 0)
                {
                    SetAlert("Thêm mới sản phẩm thành công", "success");
                }
                else
                {
                    ModelState.AddModelError("", "Thêm mới sản phẩm không thành công.");
                }

                var gallery = new Gallery
                {
                    Image1 = model.Gallery.Image1,
                    Image2 = model.Gallery.Image2,
                    Image3 = model.Gallery.Image3
                };

                long galleryId = _galleryService.Insert(gallery);
                if (galleryId > 0)
                {
                    product.GalleryId = galleryId;
                    _productService.Update(product);

                    List<ProductSize> productSizes = new List<ProductSize>();
                    foreach (var sizeStock in model.ProductSizes)
                    {
                        var productSize = new ProductSize
                        {
                            ProductId = id,
                            SizeId = sizeStock.SizeId,
                            Stock = sizeStock.Stock
                        };
                        productSizes.Add(productSize);
                    }

                    // Lưu thông tin về size và số lượng tồn vào bảng riêng
                    _productSizeService.InsertMany(productSizes);

                    SetAlert("Thêm mới ảnh thành công", "success");
                    return RedirectToAction("Index", "Product");
                }
                else
                {
                    ModelState.AddModelError("", "Thêm mới ảnh không thành công.");
                }
            }
            return View("Index");
        }

        public ActionResult Detail(long id)
        {
            var product = _productService.GetProductById(id);
            var gallery = _galleryService.GetGalleryById(product.GalleryId.Value);
            ViewBag.Product = product;
            ViewBag.ProductGallery = gallery;
            return View();
        }

        //public JsonResult LoadImages(long id)
        //{
        //    var product = _productService.GetProductById(id);
        //    var images = product.MoreImage;
        //    XElement xImages = XElement.Parse(images);
        //    List<string> listImagesReturn = new List<string>();

        //    foreach (XElement element in xImages.Elements())
        //    {
        //        listImagesReturn.Add(element.Value);
        //    }
        //    return Json(new
        //    {
        //        data = listImagesReturn
        //    }, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult SaveImages(long id, string images)
        //{
        //    JavaScriptSerializer serializer = new JavaScriptSerializer();
        //    var listImages = serializer.Deserialize<List<string>>(images);

        //    XElement xElement = new XElement("Images");

        //    foreach (var item in listImages)
        //    {
        //        var subStringItem = item.Substring(21);
        //        xElement.Add(new XElement("Image", subStringItem));
        //    }

        //    try
        //    {
        //        _productService.UpdateImages(id, xElement.ToString());
        //        return Json(new
        //        {
        //            status = true
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new
        //        {
        //            status = false
        //        });
        //    }

        //}
        public void SetViewBag(long? selectedId = null)
        {
            var productCategories = _productCategoryService.GetAll();
            var sizes = _sizeService.GetAll();
            ViewBag.CategoryId = new SelectList(productCategories, "Id", "Name", selectedId);
            ViewBag.Sizes = sizes;
        }
    }
}