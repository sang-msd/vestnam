using Data.EF;
using PagedList;
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

        public bool Delete(long id)
        {
            try
            {
                var order = db.Orders.Find(id);
                db.Orders.Remove(order);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<Order> GetAll()
        {
            return db.Orders.ToList();
        }

        public IEnumerable<Order> GetAllByDatePaging(DateTime orderDate, int page, int pageSize)
        {
            IQueryable<Order> model = db.Orders;
            var data = model.OrderBy(x => x.OrderDate).ToList();
            return data.Where(x => x.OrderDate.Date == orderDate).ToPagedList(page, pageSize);
        }

        public IEnumerable<Order> GetAllPaging(int page, int pageSize)
        {
            IQueryable<Order> model = db.Orders;
            return model.OrderBy(x => x.OrderDate).ToPagedList(page, pageSize);
        }

        public Order GetOrderById(long id)
        {
            return db.Orders.FirstOrDefault(x => x.Id == id);
        }

        public List<Order> GetOrderByUserId(long userId)
        {
            var result = from o in db.Orders
                         join u in db.Users on o.CustomerId equals u.Id
                         where u.Id == userId
                         select o;
            return result.ToList();
        }

        public long Insert(Order order)
        {
            db.Orders.Add(order);
            db.SaveChanges();
            return order.Id;
        }

        public bool Update(Order order)
        {
            try
            {
                var data = db.Orders.Find(order.Id);
                data.Name = order.Name;
                data.PaymentMethod = order.PaymentMethod;
                data.Status = order.Status;
                data.DeliveryAddress = order.DeliveryAddress;
                data.DeliveryDate = order.DeliveryDate;
                data.CustomerId = order.CustomerId;
                data.Email = order.Email;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
