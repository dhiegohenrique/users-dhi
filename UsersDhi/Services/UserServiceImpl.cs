using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersDhi.Models;

namespace UsersDhi.Services
{
    public class UserServiceImpl : IUserService
    {
        private UserContext userContext;

        public UserServiceImpl(UserContext userContext)
        {
            this.userContext = userContext;
        }

        public List<User> GetAllUsers()
        {
            return this.userContext.User.OrderBy(user => user.Id).ToList();
        }

        public async Task<User> GetUserById(int id)
        {
            return await this.userContext.User.SingleOrDefaultAsync(m => m.Id == id);
        }

        public async Task<int> InsertUpdate(User user)
        {
            var entry = this.userContext.Entry(user);

            if (user.Id == 0)
            {
                entry.State = EntityState.Added;
                user.Registerdate = DateTime.Now;
            } else
            {
                entry.State = EntityState.Modified;
                entry.Property(u => u.Username).IsModified = false;
                entry.Property(u => u.Registerdate).IsModified = false;
            }

            try
            {
                return await this.userContext.SaveChangesAsync();
            } catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Remove(int id)
        {
            try
            {
                this.userContext.User.Remove(new User { Id = id });
                return await this.userContext.SaveChangesAsync();
            } catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UserExists(int id)
        {
            return this.userContext.User.Any(e => e.Id == id);
        }

        public bool UsernameExists(string username)
        {
            int count = this.userContext.User
                .Where(user => user.Username.Equals(username))
                .Count();

            return count > 0;
        }
    }
}
