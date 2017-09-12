using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersDhi.Models;

namespace UsersDhi.Services
{
    public interface IUserService
    {
        List<User> GetAllUsers();

        Task<User> GetUserById(int id);

        Task<int> InsertUpdate(User user);

        Task<int> Remove(int id);
    }
}
