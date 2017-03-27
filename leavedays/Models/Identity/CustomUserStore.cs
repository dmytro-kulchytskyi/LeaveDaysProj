using System;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using leavedays.Models.Repository.Interfaces;

namespace leavedays.Models.Identity
{
    public class CustomUserStore : IUserStore<AppUser, int>, IUserPasswordStore<AppUser, int>, IUserLockoutStore<AppUser, int>, IUserTwoFactorStore<AppUser, int>, IUserRoleStore<AppUser, int>
    {
        private readonly IUserRepository userRepository;
        private readonly IRoleRepository roleRepository;
        public CustomUserStore(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
        }

        public Task AddToRoleAsync(AppUser user, string roleName)
        {
            roleName = roleName.ToLower();
            if (string.IsNullOrWhiteSpace(user.Roles)) user.Roles = "";
            user.Roles += "[" + roleName + "]";
            return Task.FromResult(true);
        }

        public Task CreateAsync(AppUser user)
        {
            return Task.FromResult(userRepository.Save(user));
        }

        public Task DeleteAsync(AppUser user)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            //  throw new NotImplementedException();
        }

        public Task<AppUser> FindByIdAsync(int userId)
        {
            return Task.FromResult(userRepository.GetById(userId));
        }

        public Task<AppUser> FindByNameAsync(string userName)
        {
            return Task.FromResult(userRepository.GetByUserName(userName));
        }

        public Task<int> GetAccessFailedCountAsync(AppUser user)
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(AppUser user)
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(AppUser user)
        {
            return Task.FromResult(user.LockoutEnd);
        }

        public Task<string> GetPasswordHashAsync(AppUser user)
        {
            return Task.FromResult(user.Password);
        }

        public Task<IList<string>> GetRolesAsync(AppUser user)
        {
            if (string.IsNullOrWhiteSpace(user.Roles)) return Task.FromResult(new List<string>() as IList<string>);
            return Task.FromResult(user.Roles.TrimEnd(']').Split(']').Select(role => role.TrimStart('[').ToLower()).ToList() as IList<string>);
        }

        public Task<bool> GetTwoFactorEnabledAsync(AppUser user)
        {
            return Task.FromResult(false);
        }

        public Task<bool> HasPasswordAsync(AppUser user)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.Password));
        }

        public Task<int> IncrementAccessFailedCountAsync(AppUser user)
        {
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> IsInRoleAsync(AppUser user, string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName) || string.IsNullOrWhiteSpace(user.Roles)) return Task.FromResult(false);
            roleName = roleName.ToLower();
            return Task.FromResult(user.Roles.ToLower().Contains("[" + roleName + "]"));
        }

        public Task RemoveFromRoleAsync(AppUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task ResetAccessFailedCountAsync(AppUser user)
        {
            user.AccessFailedCount = 0;
            return Task.FromResult(true);

        }

        public Task SetLockoutEnabledAsync(AppUser user, bool enabled)
        {
            user.LockoutEnabled = enabled;
            return Task.FromResult(true);
        }

        public Task SetLockoutEndDateAsync(AppUser user, DateTimeOffset lockoutEnd)
        {
            user.LockoutEnd = lockoutEnd;
            return Task.FromResult(true);
        }

        public Task SetPasswordHashAsync(AppUser user, string passwordHash)
        {
            user.Password = passwordHash;
            return Task.FromResult(true);
        }

        public Task SetTwoFactorEnabledAsync(AppUser user, bool enabled)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(AppUser user)
        {
            return Task.FromResult(userRepository.Save(user));
        }
    }
}