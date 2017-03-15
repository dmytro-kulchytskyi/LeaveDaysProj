using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using leavedays.Models;
using System.Threading.Tasks;
using leavedays.Models.Repository.Interfaces;

namespace leavedays.Models.Identity
{
    public class CustomRoleStore : IRoleStore<SiteGroup, int>
    {
        private readonly ISiteGroupRepository siteGroupRepository;
        public CustomRoleStore(ISiteGroupRepository siteGroupRepository)
        {
            this.siteGroupRepository = siteGroupRepository;
        }
        
        public Task CreateAsync(SiteGroup role)
        {
            return Task.FromResult(siteGroupRepository.Save(role));
        }

        public Task DeleteAsync(SiteGroup role)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public Task<SiteGroup> FindByIdAsync(int roleId)
        {
            return Task.FromResult(siteGroupRepository.GetById(roleId));
        }

        public Task<SiteGroup> FindByNameAsync(string roleName)
        {
            return Task.FromResult(siteGroupRepository.GetByName(roleName));
        }

        public Task UpdateAsync(SiteGroup role)
        {
            return Task.FromResult(siteGroupRepository.Save(role));
        }
    }
}