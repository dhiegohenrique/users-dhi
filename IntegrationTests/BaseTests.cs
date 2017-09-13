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
    public class BaseTests
    {
        protected HttpClient client;

        protected UserContext userContext;

        public BaseTests()
        {
            TestServer server = new TestServer(new WebHostBuilder()
           .UseContentRoot(this.GetPathProjects() + Path.DirectorySeparatorChar + "UsersDhi")
           .ConfigureServices((IServiceCollection services) => {
               services.AddDbContext<UserContext>(dbOptions =>
                    dbOptions.UseInMemoryDatabase("suntech-test"));
           })
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

        protected StringContent GetStringContent(Object obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            return new StringContent(json, UnicodeEncoding.UTF8, "application/json");
        }

        protected async Task<T> GetObjectFromHttpResponseAsync<T>(HttpResponseMessage response)
        {
            string jsonResponse = await response.Content.ReadAsStringAsync();
            return this.GetObjectFromStringResponse<T>(jsonResponse);
        }

        protected T GetObjectFromStringResponse<T>(string response)
        {
            return JsonConvert.DeserializeObject<T>(response);
        }

        protected async Task<HttpResponseMessage> PostAsync(string url, Object obj)
        {
            return await this.client.PostAsync(url, this.GetStringContent(obj));
        }

        protected async Task<HttpResponseMessage> PutAsync(string url, Object obj)
        {
            return await this.client.PutAsync(url, this.GetStringContent(obj));
        }

        protected async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            return await this.client.DeleteAsync(url);
        }

        protected async Task<T> GetAsync<T>(string url)
        {
            HttpResponseMessage response = await this.client.GetAsync(url);
            return await this.GetObjectFromHttpResponseAsync<T>(response);
        }

        protected string SerializeObject(Object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
