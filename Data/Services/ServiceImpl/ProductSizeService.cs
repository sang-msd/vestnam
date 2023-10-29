using Data.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Services.ServiceImpl
{
    public class ProductSizeService : IProductSizeService
    {
        TGClothesDbContext db = null;
        public ProductSizeService()
        {
            db = new TGClothesDbContext();
        }

        public void InsertMany(List<ProductSize> productSizes)
        {
            db.ProductSizes.AddRange(productSizes);
            db.SaveChanges();
        }
    }
}
