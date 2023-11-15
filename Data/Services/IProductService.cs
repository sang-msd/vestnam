using Data.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Services
{
    public interface IProductService
    {
        long Insert(Product product);
        bool Update(Product product);
        bool Delete(long id);
        List<Product> GetAll();
        List<Product> GetAll(ref int totalRecord, int pageIndex = 1, int pageSize = 8);
        IEnumerable<Product> GetAllPaging(string searchString, int page, int pageSize);
        Product GetProductById(long id);
        List<Product> GetProductByCategoryId(long id, ref int totalRecord, int pageIndex = 1, int pageSize = 8);
        List<Product> ListNewProduct(int top);
        List<Product> ListFeatureProduct(int top);
        List<Product> ListRelateProduct(long productId);
        void UpdateImages(long productId, string images);
        bool ChangeStatus(long id);
    }
}
