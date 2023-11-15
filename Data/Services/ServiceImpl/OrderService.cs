using Data.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Services.ServiceImpl
{
    public class OrderService : IOrderService
    {
        TGClothesDbContext db = null;
        public OrderService()
        {
            db = new TGClothesDbContext();
        }

        public long Insert(Order order)
        {
            db.Orders.Add(order);
            db.SaveChanges();
            return order.Id;
        }
    }
}
