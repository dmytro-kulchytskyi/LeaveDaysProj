using System;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using leavedays.Models.Interfaces.Repository;

namespace leavedays.Models.Identity
{
    public class CustomUserStore : IUserStore<User, int>, IUserPasswordStore<User, int>, IUserLockoutStore<User, int>, IUserTwoFactorStore<User, int>
    {
        private readonly IUserRepository repository;
        public CustomUserStore(IUserRepository userRepository)
        {
            repository = userRepository;
        }

        public Task CreateAsync(User user)
        {
            return Task.FromResult(repository.Save(user));
        }

        public Task DeleteAsync(User user)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
          //  throw new NotImplementedException();
        }

        public Task<User> FindByIdAsync(int userId)
        {
            return Task.FromResult(repository.GetById(userId));
        }

        public Task<User> FindByNameAsync(string userName)
        {
            return Task.FromResult(repository.GetByUserName(userName));
        }

        public Task<int> GetAccessFailedCountAsync(User user)
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(User user)
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(User user)
        {
            return Task.FromResult(user.LockoutEnd);
        }

        public Task<string> GetPasswordHashAsync(User user)
        {
            return Task.FromResult(user.Password);
        }

        public Task<bool> GetTwoFactorEnabledAsync(User user)
        {
            return Task.FromResult(false);
        }

        public Task<bool> HasPasswordAsync(User user)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.Password));
        }

        public Task<int> IncrementAccessFailedCountAsync(User user)
        {
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(User user)
        {
            user.AccessFailedCount = 0;
            return Task.FromResult(true);

        }

        public Task SetLockoutEnabledAsync(User user, bool enabled)
        {
            user.LockoutEnabled = enabled;
            return Task.FromResult(true);
        }

        public Task SetLockoutEndDateAsync(User user, DateTimeOffset lockoutEnd)
        {
            user.LockoutEnd = lockoutEnd;
            return Task.FromResult(true);
        }

        public Task SetPasswordHashAsync(User user, string passwordHash)
        {
            user.Password = passwordHash;
            return Task.FromResult(true);
        }

        public Task SetTwoFactorEnabledAsync(User user, bool enabled)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(User user)
        {
            return Task.FromResult(repository.Save(user));
        }
    }
}