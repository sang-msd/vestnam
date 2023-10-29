using Data.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Services
{
    public interface IRateService
    {
        List<Rate> GetAll();
        List<Rate> GetRateByProductId(long id);
        Rate Insert(Rate rate);
    }
}
