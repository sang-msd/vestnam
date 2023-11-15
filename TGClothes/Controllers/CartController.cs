using Common;
using Data.EF;
using Data.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TGClothes.Common;
using TGClothes.Models;

namespace TGClothes.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly ISizeService _sizeService;
        private readonly IProductSizeService _productSizeService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IOrderService _orderService;

        public CartController(
            IProductService productService, 
            ISizeService sizeService, 
            IProductSizeService productSizeService, 
            IOrderDetailService orderDetailService, 
            IOrderService orderService)
        {
            _productService = productService;
            _sizeService = sizeService;
            _productSizeService = productSizeService;
            _orderDetailService = orderDetailService;
            _orderService = orderService;
        }

        // GET: Cart
        public ActionResult Index()
        {
            var cart = Session[CommonConstants.CartSession];
            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;
            }
            ViewBag.SubTotal = SubTotal();
            ViewBag.Promotion = Promotion();
            return View(list);
        }

        public ActionResult AddToCart(long productId, long sizeId, int quantity)
        {
            var product = _productService.GetProductById(productId);
            var size = _sizeService.GetSizeById(sizeId);
            var cart = Session[CommonConstants.CartSession];
            if (cart != null)
            {
                var list = (List<CartItem>)cart;
                if (list.Exists(x => x.Product.Id == productId && x.Size.Id == sizeId))
                {
                    foreach (var item in list)
                    {
                        var stock = _productSizeService.GetStock(item.Product.Id, item.Size.Id);
                        if (item.Product.Id == productId && item.Size.Id == sizeId)
                        {
                            if (item.Quantity < stock)
                            {
                                item.Quantity += quantity;
                            }
                            
                            else
                            {
                                TempData["msg"] = "<script>alert('Sản phẩm k được vượt quá số lượng tồn');</script>";
                            }
                        }
                    }
                }
                else
                {
                    //tao moi doi tuong cart item
                    var item = new CartItem();
                    item.Product = product;
                    item.Size = size;
                    item.Quantity = quantity;
                    list.Add(item);
                }
                //gan vao session
                Session[CommonConstants.CartSession] = list;
            }
            else
            {
                //tao moi doi tuong cart item
                var item = new CartItem();
                item.Product = product;
                item.Size = size;
                item.Quantity = quantity;
                var list = new List<CartItem>();
                list.Add(item);

                //gan vao session
                Session[CommonConstants.CartSession] = list;
            }
            return Redirect(Request.UrlReferrer.ToString());
        }

        public JsonResult Update(string cartModel)
        {
            var jsonCart = new JavaScriptSerializer().Deserialize<List<CartItem>>(cartModel);
            var sessionCart = (List<CartItem>)Session[CommonConstants.CartSession];
            

            foreach (var item in sessionCart)
            {
                var stock = _productSizeService.GetStock(item.Product.Id, item.Size.Id);
                var jsonItem = jsonCart.SingleOrDefault(x => x.Product.Id == item.Product.Id && x.Size.Id == item.Size.Id);
                if (jsonItem != null && jsonItem.Quantity <= stock)
                {
                    item.Quantity = jsonItem.Quantity;
                }
                else
                {
                    TempData["msg"] = "<script>alert('Sản phẩm k được vượt quá số lượng tồn');</script>";
                }
            }
            Session[CommonConstants.CartSession] = sessionCart;
            return Json(new
            {
                status = true
            });
        }

        public JsonResult Delete(long productId, long sizeId)
        {
            var sessionCart = (List<CartItem>)Session[CommonConstants.CartSession];
            sessionCart.RemoveAll(x => x.Product.Id == productId && x.Size.Id == sizeId);
            Session[CommonConstants.CartSession] = sessionCart;
            return Json(new
            {
                status = true
            });
        }

        public JsonResult DeleteAll()
        {
            Session[CommonConstants.CartSession] = null;
            return Json(new
            {
                status = true
            });
        }

        public ActionResult Payment()
        {
            var cart = Session[CommonConstants.CartSession];
            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;
            }
            ViewBag.SubTotal = SubTotal();
            ViewBag.Promotion = Promotion();
            return View(list);
        }

        [HttpPost]
        public ActionResult Payment(string name, string email, string phone, string address)
        {
            var order = new Order();
            order.OrderDate = DateTime.Now;
            order.Name = name;
            order.Email = email;
            order.Phone = phone;
            order.DeliveryAddress = address;
            order.Status = (int)OrderStatus.PENDING;
            order.PaymentMethod = (int)PaymentMethods.COD;

            var id = _orderService.Insert(order);
            var cart = (List<CartItem>)Session[CommonConstants.CartSession];
            foreach (var item in cart)
            {
                var orderDetail = new OrderDetail();
                orderDetail.ProductId = item.Product.Id;
                orderDetail.OrderId = id;
                orderDetail.SizeId = item.Size.Id;
                orderDetail.Price = item.Product.PromotionPrice ?? item.Product.Price;
                orderDetail.Quantity = item.Quantity;
                orderDetail.TotalQuantity = TotalQuantity();
                orderDetail.TotalPrice = Total();

                var stock = _productSizeService.GetProductSizeByProductIdAndSizeId(item.Product.Id, item.Size.Id);
                stock.Stock -= orderDetail.Quantity;
                _productSizeService.Update(stock);

                _orderDetailService.Insert(orderDetail);
            }
            string content = System.IO.File.ReadAllText(Server.MapPath("~/Assets/Client/template/neworder.html"));
            content = content.Replace("{{CustomerName}}", name);
            content = content.Replace("{{Phone}}", phone);
            content = content.Replace("{{Email}}", email);
            content = content.Replace("{{Address}}", address);
            content = content.Replace("{{Total}}", Total().ToString("N0"));
            var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"];

            new MailHelper().SendMail(email, "Đơn hàng mới từ TGClothes", content);
            new MailHelper().SendMail(toEmail, "Đơn hàng mới từ TGClothes", content);

            return Redirect("/hoan-thanh");
        }

        public ActionResult OrderSuccess()
        {
            return View();
        }

        [ChildActionOnly]
        public PartialViewResult CartModel()
        {
            var cart = Session[CommonConstants.CartSession];
            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;
            }
            return PartialView(list);
        }

        private decimal SubTotal()
        {
            decimal subtotal = 0;
            List<CartItem> cart = Session[CommonConstants.CartSession] as List<CartItem>;
            if (cart != null)
            {
                subtotal = cart.Sum(n => n.Quantity * n.Product.Price.Value);
            }
            return subtotal;
        }

        private decimal Promotion()
        {
            decimal promotion = 0;
            List<CartItem> cart = Session[CommonConstants.CartSession] as List<CartItem>;
            if (cart != null)
            {
                promotion = cart.Sum(n => n.Quantity * n.Product.Price.Value * (n.Product.Promotion.HasValue ? n.Product.Promotion.Value : 0) / 100);
            }
            return promotion;
        }

        private int TotalQuantity()
        {
            int total = 0;
            List<CartItem> cart = Session[CommonConstants.CartSession] as List<CartItem>;
            if (cart != null)
            {
                total = cart.Sum(n => n.Quantity);
            }
            return total;
        }

        private decimal Total()
        {
            decimal total = 0;
            List<CartItem> cart = Session[CommonConstants.CartSession] as List<CartItem>;
            if (cart != null)
            {
                foreach (var item in cart)
                {
                    decimal itemPrice = item.Product.Price.Value;

                    if (item.Product.PromotionPrice != null)
                    {
                        itemPrice = item.Product.PromotionPrice.Value;
                    }

                    total += item.Quantity * itemPrice;
                }
            }
            return total;
        }
    }
}