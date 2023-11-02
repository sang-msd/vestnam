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
        IEnumerable<Product> GetAllPaging(string searchString, int page, int pageSize);
        Product GetProductById(long id);
        List<Product> ListNewProduct(int top);
        List<Product> ListFeatureProduct(int top);
        List<Product> ListRelateProduct(long productId);
        void UpdateImages(long productId, string images);
        bool ChangeStatus(long id);
    }
}
