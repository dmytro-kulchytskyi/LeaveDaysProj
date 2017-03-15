using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace leavedays.Models.Identity
{
    public class CustomUserManager : UserManager<User, int>
    {
        public CustomUserManager(IUserStore<User, int> store) : base(store)
        {
        }
    }
}