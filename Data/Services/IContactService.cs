using Data.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Services
{
    public interface IContactService
    {
        Contact GetActiveContact();
        long Insert(Contact contact);
        bool Update(Contact contact);
        bool Delete(long id);
    }
}
