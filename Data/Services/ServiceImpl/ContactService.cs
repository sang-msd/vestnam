using Data.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Services.ServiceImpl
{
    public class ContactService : IContactService
    {
        TGClothesDbContext db = null;
        public ContactService()
        {
            db = new TGClothesDbContext();
        }

        public bool Delete(long id)
        {
            throw new NotImplementedException();
        }

        public Contact GetActiveContact()
        {
            return db.Contacts.SingleOrDefault(x => x.Status == true);
        }

        public long Insert(Contact contact)
        {
            throw new NotImplementedException();
        }

        public bool Update(Contact contact)
        {
            throw new NotImplementedException();
        }
    }
}
