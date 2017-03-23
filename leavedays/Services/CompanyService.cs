using leavedays.Models;
using leavedays.Models.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace leavedays.Services
{
    public class CompanyService
    {
        public CompanyService(IUserRepository userRepository, IRoleRepository roleRepository, ICompanyRepository companyRepository)
        {
            this.companyRepository = companyRepository;
            this.roleRepository = roleRepository;
            this.userRepository = userRepository;
        }
        private readonly ICompanyRepository companyRepository;
        private readonly IRoleRepository roleRepository;
        private readonly IUserRepository userRepository;

        public string[] GetRolesFromLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return new string[0];
            line = line.Trim(',');
            var roles = line.Split(',');
            if (roles.Length == 0)
                return new string[0];
            else return roles;
        }
        public string GetRolesLine(IEnumerable<string> roles)
        {
            StringBuilder rolesLine = new StringBuilder();
            foreach (var role in roles)
            {
                rolesLine.Append("[" + role + "]");
            }
            return rolesLine.ToString();
        }

        public AppUser GetUserByName(string name)
        {
            return userRepository.GetByUserName(name);
        }
        public AppUser GetUserById(int id)
        {
            return userRepository.GetById(id);
        }

        public int SaveCompany(Company company)
        {
            return companyRepository.Save(company);
        }

    }
}