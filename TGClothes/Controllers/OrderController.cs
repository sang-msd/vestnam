using Data.EF;
using Data.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGClothes.Common;
using TGClothes.Models;

namespace TGClothes.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IProductSizeService _productSizeService;
        private readonly ISizeService _sizeService;
        private readonly IRateService _rateService;

        public OrderController(
            IOrderService orderService, 
            IOrderDetailService orderDetailService,
            IUserService userService, 
            IProductService productService,
            IProductSizeService productSizeService,
            ISizeService sizeService, 
            IRateService rateService)
        {
            _orderService = orderService;
            _orderDetailService = orderDetailService;
            _userService = userService;
            _productService = productService;
            _productSizeService = productSizeService;
            _sizeService = sizeService;
            _rateService = rateService;
        }
        // GET: Order
        public ActionResult Index()
        {
            var user = (UserLogin)Session[CommonConstants.USER_SESSION];
            var model = _orderService.GetOrderByUserId(user.UserId);
            return View(model);
        }

        public ActionResult Details(long id)
        {
            var orderDetail = _orderDetailService.GetOrderDetailByOrderId(id);
            var data = from od in _orderDetailService.GetAll()
                       join p in _productService.GetAll() on od.ProductId equals p.Id
                       join o in _orderService.GetAll() on od.OrderId equals o.Id
                       join s in _sizeService.GetAll() on od.SizeId equals s.Id
                       where od.OrderId == id && p.Id == od.ProductId && od.OrderId == o.Id
                       select new OrderDetailModel
                       {
                           Product = p,
                           Order = o,
                           OrderDetail = od,
                           Size = s
                       };

            ViewBag.OrderDetails = data;
            return View(orderDetail);
        }

        [HttpDelete]
        public ActionResult CancelOrder(long id)
        {
            try
            {
                var order = _orderService.GetOrderById(id);
                var orderDetail = _orderDetailService.GetOrderDetailByOrderId(id);

                if (order.Status == (int)OrderStatus.PENDING)
                {
                    order.DeliveryDate = DateTime.Now;
                    order.Status = (int)OrderStatus.CANCELLED;
                    _orderService.Update(order);
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["thongbao"] = "<script>alert('Đơn hàng đang được đang xử lý không thể hủy');</script>";

                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                return View("Error" + e);
            }
        }

        [HttpPost]
        public ActionResult ProductReviews(Rate review, int rating, string content)
        {
            var user = (UserLogin)Session[CommonConstants.USER_SESSION];
            review.CreatedDate = DateTime.Now;
            review.Content = content;
            review.UserId = _userService.GetUserByEmail(user.Email).Id;
            review.Star = rating;
            review.ProductId = (long)Session["ProductId"];

            _rateService.Insert(review);
            return RedirectToAction("Details", "Order", new { id = review.ProductId });
        }
    }
}