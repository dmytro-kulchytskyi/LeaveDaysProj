using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace leavedays.Models.Interfaces.Repository
{
    public interface IUserRepository
    {
        int Save(AppUser user);
        AppUser GetById(int id);
        AppUser GetByUserName(string userName);
        IList<AppUser> GetAll();   
    }
}
