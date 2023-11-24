using Data.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Services.ServiceImpl
{
    public class OrderDetailService : IOrderDetailService
    {
        TGClothesDbContext db = null;
        public OrderDetailService()
        {
            db = new TGClothesDbContext();
        }

        public List<OrderDetail> GetAll()
        {
            return db.OrderDetails.ToList();
        }

        public bool Insert(OrderDetail detail)
        {
            try
            {
                db.OrderDetails.Add(detail);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<OrderDetail> GetOrderDetailByOrderId(long id)
        {
            return (from od in db.OrderDetails
                    join o in db.Orders on od.OrderId equals o.Id
                    where o.Id == id
                    select od).ToList();
        }

        public bool Delete(OrderDetail orderDetail)
        {
            try
            {
                db.OrderDetails.Remove(orderDetail);
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