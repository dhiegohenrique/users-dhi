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
    public class UsersTests : BaseTests
    {
        private string baseUrl = "/api/user";

        private List<User> listUsers;

        [TestMethod]
        public void ShouldBeReturnStatus201WhenInsertingNewUser()
        {
            User user = new User
            {
                Name = "newUser",
                Password = "newSenha",
                Phone = "9999999999",
                Surname = "newSobrenome",
                Username = "newUsername",
                Email = "newEmail@hotmail.com"
            };

            Task.WaitAll(Task.Run(async () =>
            {
                HttpResponseMessage response = await this.PostAsync(baseUrl, user);
                User userResponse = await this.GetObjectFromHttpResponseAsync<User>(response);

                Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
                Assert.AreNotEqual(0, userResponse.Id);
            }));
        }

        [TestMethod]
        public void ShouldBeReturnStatus500WhenInsertingNewUserAndUsernameExists()
        {
            User user = new User
            {
                Name = "newUser",
                Password = "newSenha",
                Phone = "9999999999",
                Surname = "newSobrenome",
                Username = this.listUsers[0].Username,
                Email = "newEmail@hotmail.com"
            };

            Task.WaitAll(Task.Run(async () =>
            {
                HttpResponseMessage response = await this.PostAsync(baseUrl, user);
                Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            }));
        }

        [TestMethod]
        public void ShouldBeReturnStatus400WhenInsertingNewUserAndRequiredFieldIsBlank()
        {
            User user = new User
            {
                Name = "newUser",
                Password = "newSenha",
                Phone = "9999999999",
                Surname = "newSobrenome",
                Email = "newEmail@hotmail.com"
            };

            Task.WaitAll(Task.Run(async () =>
            {
                HttpResponseMessage response = await this.PostAsync(baseUrl, user);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }));
        }

        [TestMethod]
        public void ShouldBeReturnStatus204WhenEditingUser()
        {
            User user = this.listUsers[0];
            user.Name = user.Name + " - editado";

            Task.WaitAll(Task.Run(async () =>
            {
                HttpResponseMessage response = await this.PutAsync(baseUrl + "/" + user.Id, user);
                Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            }));
        }

        [TestMethod]
        public void ShouldBeReturnStatus400WhenEditingUserAndRequiredFieldsIsBlank()
        {
            User user = this.listUsers[0];
            user.Username = null;

            Task.WaitAll(Task.Run(async () =>
            {
                HttpResponseMessage response = await this.PutAsync(baseUrl + "/" + user.Id, user);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }));
        }

        [TestMethod]
        public void ShouldBeReturnStatus400WhenEditingUserAndIDNotBelongUser()
        {
            User user = this.listUsers[0];
            user.Name = user.Name + " - editado";

            Task.WaitAll(Task.Run(async () =>
            {
                HttpResponseMessage response = await this.PutAsync(baseUrl + "/" + this.listUsers[1].Id, user);
                Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            }));
        }

        [TestMethod]
        public void ShouldBeReturnStatus404WhenEditingAUserNotFound()
        {
            User user = this.listUsers[0];
            user.Name = user.Name + " - editado";
            user.Id = 999;

            Task.WaitAll(Task.Run(async () =>
            {
                HttpResponseMessage response = await this.PutAsync(baseUrl + "/" + user.Id, user);
                Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            }));
        }

        [TestMethod]
        public void ShouldBeReturnStatus204WhenDeletingAUser()
        {
            User user = this.listUsers[0];

            Task.WaitAll(Task.Run(async () =>
            {
                HttpResponseMessage response = await this.DeleteAsync(baseUrl + "/" + user.Id);
                Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            }));
        }

        [TestMethod]
        public void ShouldBeReturnStatus404WhenDeletingAUserNotFound()
        {
            User user = this.listUsers[0];
            user.Id = 999;

            Task.WaitAll(Task.Run(async () =>
            {
                HttpResponseMessage response = await this.DeleteAsync(baseUrl + "/" + user.Id);
                Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            }));
        }

        [TestMethod]
        public void ShouldBeReturnAllUsers()
        {
            Task.WaitAll(Task.Run(async () =>
            {
                List<User> listUsers = await this.GetAsync<List<User>>(this.baseUrl);
                Assert.AreEqual(this.listUsers.Count, listUsers.Count);

                listUsers.ForEach((user) =>
                {
                    Assert.AreNotEqual(0, user.Id);
                });
            }));
        }

        [TestMethod]
        public void ShouldBeReturnUserById()
        {
            User user = this.listUsers[0];

            Task.WaitAll(Task.Run(async () =>
            {
                User userResponse = await this.GetAsync<User>(this.baseUrl + "/" + user.Id);
                Assert.AreEqual(this.SerializeObject(user), this.SerializeObject(userResponse));
            }));
        }

        /*[TestMethod]
        public void ShouldBeTestWhere()
        {
            string nome = "nome0";

            Task.WaitAll(Task.Run(async () =>
            {
                User user = await this.userContext.User.SingleOrDefaultAsync(u => u.Name.ToLower().Contains(nome.ToLower()));
                if (user == null)
                {
                    Console.WriteLine("usuário não encontrado");
                } else
                {
                    Console.WriteLine("usuário encontrado");
                }
            }));
        }*/

        [TestInitialize]
        public void FillInUsers()
        {
            this.listUsers = new List<User>();
            for (int index = 0; index < 5; index++)
            {
                this.listUsers.Add(this.CreateUser(index));
            }

            this.userContext.User.RemoveRange(this.userContext.User);
            this.userContext.AddRange(this.listUsers);
            Task.WaitAll(Task.Run(async () =>
            {
                await this.userContext.SaveChangesAsync();
            }));
        }

        private User CreateUser(int index)
        {
            User user = new User
            {
                Name = "Nome" + index + " teste da silva",
                Password = "senha" + index,
                Phone = "111111111" + index,
                Registerdate = DateTime.Now,
                Surname = "Sobrenome" + index,
                Username = "Username" + index,
                Email = "Email" + index + "@hotmail.com"
            };

            return user;
        }

    }
}
