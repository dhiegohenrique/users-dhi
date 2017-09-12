using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UsersDhi.Models;

namespace IntegrationTests
{
    [TestClass]
    public class UsersTests
    {
        private HttpClient client;

        private string baseUrl = "/api/user";

        private List<User> listUsers;

        private UserContext userContext;

        public UsersTests() {
            TestServer server = new TestServer(new WebHostBuilder()
           .UseContentRoot(this.GetPathProjects() + Path.DirectorySeparatorChar + "UsersDhi")
           .ConfigureServices((IServiceCollection services) => {
               services.AddDbContext<UserContext>(dbOptions =>
                    dbOptions.UseInMemoryDatabase("suntech-test"));
           })
           /*.Configure((IApplicationBuilder app) => {
               app.UseMvc();
           })*/
           .UseEnvironment("test")
           .UseStartup<UsersDhi.Startup>());
            this.client = server.CreateClient();
            this.userContext = server.Host.Services.GetService<UserContext>();
        }

        private string GetPathProjects()
        {
            string projectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            return projectPath.Remove(projectPath.LastIndexOf(Path.DirectorySeparatorChar));
        }

        [TestMethod]
        public void ShouldBeReturnAllUsers()
        {
            Task.WaitAll(Task.Run(async () =>
            {
                HttpResponseMessage response = await this.client.GetAsync(this.baseUrl);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                List<User> listUsers = JsonConvert.DeserializeObject<List<User>>(jsonResponse);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual(this.listUsers.Count, listUsers.Count);

                foreach (User user in listUsers)
                {
                    Assert.AreNotEqual(0, user.id);
                }
            }));
        }

        [TestMethod]
        public void ShouldBeReturnUserById()
        {
            User user = this.listUsers[0];

            Task.WaitAll(Task.Run(async () =>
            {
                HttpResponseMessage response = await this.client.GetAsync(this.baseUrl + "/" + user.id);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                User userResponse = this.GetUserFromResponse(jsonResponse);

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual(JsonConvert.SerializeObject(user), JsonConvert.SerializeObject(userResponse));
            }));
        }

        [TestMethod]
        public void ShouldBeInsertNewUser()
        {
            User user = new User
            {
                name = "newUser",
                password = "newSenha",
                phone = "9999999999",
                registerdate = DateTime.Now,
                surname = "newSobrenome",
                username = "newUsername",
                email = "newEmail@hotmail.com"
            };

            Task.WaitAll(Task.Run(async () =>
            {
                HttpResponseMessage response = await this.client.PostAsync(baseUrl, this.GetStringContent(user));
                string jsonResponse = await response.Content.ReadAsStringAsync();
                User userResponse = this.GetUserFromResponse(jsonResponse);

                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
                Assert.AreNotEqual(0, userResponse.id);
            }));
        }

        [TestMethod]
        public void ShouldBeUpdateUser()
        {
            User user = this.listUsers[0];
            user.name = user.name + " - editado";
            
            Task.WaitAll(Task.Run(async () =>
            {
                HttpResponseMessage response = await this.client.PutAsync(baseUrl + "/" + user.id, this.GetStringContent(user));
                Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            }));
        }

        [TestMethod]
        public void ShouldBeDeleteUser()
        {
            User user = this.listUsers[0];
            
            Task.WaitAll(Task.Run(async () =>
            {
                HttpResponseMessage response = await this.client.DeleteAsync(baseUrl + "/" + user.id);
                Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            }));
        }

        [TestMethod]
        public void ShouldBeTestWhere()
        {
            string nome = "nome0";

            Task.WaitAll(Task.Run(async () =>
            {
                User user = await this.userContext.User.SingleOrDefaultAsync(u => u.name.ToLower().Contains(nome.ToLower()));
                if (user == null)
                {
                    Console.WriteLine("usuário não encontrado");
                } else
                {
                    Console.WriteLine("usuário encontrado");
                }
            }));
        }

        private StringContent GetStringContent(User user)
        {
            string json = JsonConvert.SerializeObject(user);
            return new StringContent(json, UnicodeEncoding.UTF8, "application/json");
        }

        private User GetUserFromResponse(string response)
        {
            return JsonConvert.DeserializeObject<User>(response);
        }

        [TestInitialize]
        public void FillInUsers()
        {
            this.listUsers = new List<User>();

            for (int index = 0; index < 5; index++)
            {
                User user = new User
                {
                    name = "Nome" + index + " teste da silva",
                    password = "senha" + index,
                    phone = "111111111" + index,
                    registerdate = DateTime.Now,
                    surname = "Sobrenome" + index,
                    username = "username" + index,
                    email = "email" + index + "@hotmail.com"
                };
                this.listUsers.Add(user);
            }

            this.userContext.User.RemoveRange(this.userContext.User);
            this.userContext.AddRange(this.listUsers);
            Task.WaitAll(Task.Run(async () =>
            {
                await this.userContext.SaveChangesAsync();
            }));
        }
    }
}
