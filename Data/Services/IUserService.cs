using Data.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;

namespace Data.Services
{
    public interface IUserService
    {
        long Insert(User user);
        long InsertForFacebook(User user);
        bool Update(User user);
        IEnumerable<User> GetAllPaging(string searchString, int page, int pageSize);
        List<User> GetAll();
        User GetUserById(long id);
        User GetUserByName(string userName);
        User GetUserByEmail(string email);
        User GetUserByResetPasswordCode(string resetPass);
        bool Delete(long id);
        int Login(string userName, string password);
        int LoginByEmail(string email, string password);
        bool ChangeStatus(long id);
        bool CheckEmailExist(string email);
    }
}
