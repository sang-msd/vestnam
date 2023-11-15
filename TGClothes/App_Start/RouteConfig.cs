using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TGClothes
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.IgnoreRoute("{*botdetect}", new { botdetect = @"(.*)BotDetectCaptcha\.ashx" });

            routes.MapRoute(
                name: "Product",
                url: "san-pham",
                defaults: new { controller = "Product", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "TGClothes.Controllers" }
            );

            routes.MapRoute(
                name: "About",
                url: "gioi-thieu",
                defaults: new { controller = "About", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "TGClothes.Controllers" }
            );

            routes.MapRoute(
                name: "Contact",
                url: "lien-he",
                defaults: new { controller = "Contact", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "TGClothes.Controllers" }
            );

            //routes.MapRoute(
            //    name: "Product Category",
            //    url: "san-pham/{metatitle}-{productCategoryId}",
            //    defaults: new { controller = "Product", action = "Category", id = UrlParameter.Optional },
            //    namespaces: new[] { "TGClothes.Controllers" }
            //);

            routes.MapRoute(
                name: "Product Category",
                url: "san-pham/{metatitle}-{id}",
                defaults: new { controller = "Product", action = "ProductByCategory", id = UrlParameter.Optional },
                namespaces: new[] { "TGClothes.Controllers" }
            );

            routes.MapRoute(
                name: "Product Details",
                url: "chi-tiet/{metatitle}-{Id}",
                defaults: new { controller = "Product", action = "Detail", id = UrlParameter.Optional },
                namespaces: new[] { "TGClothes.Controllers" }
            );

            routes.MapRoute(
                name: "Cart",
                url: "gio-hang",
                defaults: new { controller = "Cart", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "TGClothes.Controllers" }
            );

            routes.MapRoute(
                name: "Payment",
                url: "thanh-toan",
                defaults: new { controller = "Cart", action = "Payment", id = UrlParameter.Optional },
                namespaces: new[] { "TGClothes.Controllers" }
            );

            routes.MapRoute(
                name: "Add To Cart",
                url: "them-gio-hang",
                defaults: new { controller = "Cart", action = "AddToCart", id = UrlParameter.Optional },
                namespaces: new[] { "TGClothes.Controllers" }
            );

            routes.MapRoute(
                name: "Order Sussess",
                url: "hoan-thanh",
                defaults: new { controller = "Cart", action = "OrderSuccess", id = UrlParameter.Optional },
                namespaces: new[] { "TGClothes.Controllers" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "TGClothes.Controllers" }
            );
        }
    }
}
