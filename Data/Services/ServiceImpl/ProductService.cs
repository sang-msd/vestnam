using Data.EF;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Data.Services.ServiceImpl
{
    public class ProductService : IProductService
    {
        TGClothesDbContext db = null;
        public ProductService()
        {
            db = new TGClothesDbContext();
        }

        public List<Product> GetAll()
        {
            return db.Products.ToList();
        }

        public IEnumerable<Product> GetAllPaging(string searchString, int page, int pageSize)
        {
            IQueryable<Product> model = db.Products;
            if (!string.IsNullOrEmpty(searchString))
            {
                model = model.Where(x => x.Name.Contains(searchString));
            }
            return model.OrderBy(x => x.CreatedDate).ToPagedList(page, pageSize);
        }

        public Product GetProductById(long id)
        {
            return db.Products.Find(id);
        }

        public long Insert(Product product)
        {
            var metaTitle = product.Name.Trim();
            metaTitle = Regex.Replace(metaTitle, @"[^a-z0-9\s-]", "");

            // Thay thế các khoảng trắng bằng dấu gạch ngang
            metaTitle = Regex.Replace(metaTitle, @"\s+", "-");
            product.MetaTitle = metaTitle;
            db.Products.Add(product);
            db.SaveChanges();
            return product.Id;
        }

        public List<Product> ListFeatureProduct(int top)
        {
            return db.Products.Where(x => x.TopHot != null && x.TopHot > DateTime.Now).OrderByDescending(x => x.CreatedDate).Take(top).ToList();
        }

        public List<Product> ListNewProduct(int top)
        {
            return db.Products.OrderByDescending(x => x.CreatedDate).Take(top).ToList();
        }

        public List<Product> ListRelateProduct(long productId)
        {
            var product = db.Products.Find(productId);
            return db.Products.Where(x => x.Id != productId && x.CategoryId == product.CategoryId).ToList();
        }

        public bool Update(Product product)
        {
            var data = db.Products.Find(product.Id);
            if (data != null)
            {
                data.Name = product.Name;
                data.MetaTitle = product.MetaTitle;
                data.Description = product.Description;
                data.Image = product.Image;
                data.Price = product.Price;
                data.Promotion = product.Promotion;
                data.PromotionPrice = product.PromotionPrice;
                data.CategoryId = product.CategoryId;
                data.MetaKeywords = product.MetaKeywords;
                data.MetaDescription = product.MetaDescription;
                data.TopHot = product.TopHot;
                data.ViewCount = product.ViewCount;
                data.GalleryId = product.GalleryId;

                // Lưu các thay đổi vào cơ sở dữ liệu
                db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void UpdateImages(long productId, string images)
        {
            //var product = db.Products.Find(productId);
            //product.MoreImage = images;
            //db.SaveChanges();
        }
    }
}
