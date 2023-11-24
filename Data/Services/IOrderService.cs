using Data.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Services
{
    public interface IOrderService
    {
        long Insert(Order order);
        bool Update(Order order);
        List<Order> GetAll();
        IEnumerable<Order> GetAllPaging(int page, int pageSize);
        IEnumerable<Order> GetAllByDatePaging(DateTime orderDate, int page, int pageSize);
        Order GetOrderById(long id);
        List<Order> GetOrderByUserId(long userId);
        bool Delete(long id);
    }
}
