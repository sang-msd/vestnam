using Data.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGClothes.Models
{
    public class ProductImageModel
    {
        public Product Product { get; set; }
        public Gallery Gallery { get; set; }
        public List<Size> Sizes { get; set; }
        public List<ProductSize> ProductSizes { set; get; }
    }
}