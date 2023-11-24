using Data.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Services
{
    public interface IOrderDetailService
    {
        bool Insert(OrderDetail detail);
        List<OrderDetail> GetAll();
        List<OrderDetail> GetOrderDetailByOrderId(long id);
        bool Delete(OrderDetail orderDetail);
    }
}
