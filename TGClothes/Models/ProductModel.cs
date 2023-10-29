using Data.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGClothes.Models
{
    public class ProductModel
    {
        public List<Product> Products { get; set; }
        public List<Gallery> Galleries { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }
    }
}