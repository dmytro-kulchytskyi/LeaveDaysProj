using leavedays.Models;
using leavedays.Models.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public string GetRolesFromLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return "";
            line = line.TrimEnd(',');
            var roles = line.Split(',');
            if (roles.Length == 0)
                return "";
            else
            {
                var allRoles = roleRepository.GetAll();
                foreach (var role in allRoles)
                {
                    if (!roles.Contains(role.Name)) allRoles.Remove(role);
                }
                return "," + string.Join(",", allRoles.Select(r => r.Name)) + ",";
            }
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