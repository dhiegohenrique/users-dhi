using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication23.Models;

namespace UnitTestProject1.Services
{
    public interface IUserService
    {
        List<User> GetAllUsers();

        Task<User> GetUserById(int id);

        Task<int> InsertUpdate(User user);

        Task<int> Remove(int id);
    }

}
