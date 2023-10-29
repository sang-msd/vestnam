using Data.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Services.ServiceImpl
{
    public class CategoryService : ICategoryService
    {
        TGClothesDbContext db = null;
        public CategoryService()
        {
            db = new TGClothesDbContext();
        }

        public List<Category> GetAll()
        {
            return db.Categories.Where(x => x.Status == true).ToList();
        }
    }
}
