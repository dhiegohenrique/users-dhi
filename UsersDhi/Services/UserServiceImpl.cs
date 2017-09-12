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
            return this.userContext.User.OrderBy(user => user.id).ToList();
        }

        public async Task<User> GetUserById(int id)
        {
            return await this.userContext.User.SingleOrDefaultAsync(m => m.id == id);
        }

        public async Task<int> InsertUpdate(User user)
        {
            var entry = this.userContext.Entry(user);

            if (user.id == 0)
            {
                entry.State = EntityState.Added;
            } else 
            {
                entry.State = EntityState.Modified;
            }

            try
            {
                return await this.userContext.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException ex)
            {
                if (!UserExists(user.id))
                {
                    throw new DbUpdateException("User not found", ex);
                }

                throw ex;
            } catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Remove(int id)
        {
            if (!UserExists(id))
            {
                throw new Exception("User not found");
            }

            this.userContext.User.Remove(new User { id = id });
            return await this.userContext.SaveChangesAsync();
        }

        private bool UserExists(int id)
        {
            return this.userContext.User.Any(e => e.id == id);
        }
    }
}
