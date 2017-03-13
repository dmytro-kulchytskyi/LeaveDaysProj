using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace leavedays.Models.Interfaces.Repository
{
    public interface IUserRepository
    {
        int Save(User user);
        User GetById(int id);
        User GetByUserName(string userName);
        IList<User> GetAll();   
    }
}
