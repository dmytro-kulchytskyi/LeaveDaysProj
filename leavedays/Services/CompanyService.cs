using leavedays.Models.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace leavedays.Services
{
    public class CompanyService
    {
        public CompanyService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        private readonly IUserRepository userRepository;


    }
}