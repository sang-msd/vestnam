using Common;
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

        public bool ChangeStatus(long id)
        {
            var product = db.Products.Find(id);
            product.Status = !product.Status;
            db.SaveChanges();
            return product.Status;
        }

        public bool Delete(long id)
        {
            try
            {
                var product = db.Products.Find(id);
                db.Products.Remove(product);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<Product> GetAll(ref int totalRecord, int pageIndex = 1, int pageSize = 8)
        {
            totalRecord = db.Products.Count();
            return db.Products.OrderByDescending(x => x.CreatedDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
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

        public List<Product> GetProductByCategoryId(long id, ref int totalRecord, int pageIndex = 1, int pageSize = 8)
        {
            totalRecord = db.Products.Where(x => x.CategoryId == id).Count();
            return db.Products.Where(x => x.CategoryId == id).OrderByDescending(x => x.CreatedDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public Product GetProductById(long id)
        {
            return db.Products.Find(id);
        }

        public long Insert(Product product)
        {
            //Xử lý alias
            if (string.IsNullOrEmpty(product.MetaTitle))
            {
                product.MetaTitle = StringHelper.ToUnsignString(product.Name);
            }
            db.Products.Add(product);
            db.SaveChanges();
            return product.Id;
        }

        public List<Product> ListFeatureProduct(int top)
        {
            return db.Products.Where(x => x.TopHot != null && x.TopHot > DateTime.Now).OrderByDescending(x => x.CreatedDate).Take(top).ToList();
        }

        public List<string> ListName(string keyword)
        {
            return db.Products.Where(x => x.Name.Contains(keyword)).Select(x => x.Name).ToList();
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

        public List<Product> Search(string searchkeyword, ref int totalRecord, int pageIndex = 1, int pageSize = 8)
        {
            totalRecord = db.Products.Where(x => x.Name.Contains(searchkeyword)).Count();
            return db.Products.Where(x => x.Name.Contains(searchkeyword)).OrderByDescending(x => x.CreatedDate).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public bool Update(Product product)
        {
            var data = db.Products.Find(product.Id);
            if (data != null)
            {
                data.Name = product.Name;
                data.MetaTitle = StringHelper.ToUnsignString(product.Name); ;
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
    }
}
