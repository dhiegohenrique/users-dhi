using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using UnitTestProject1.Services;
using WebApplication1.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.ResponseCompression;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            CurrentEnvironment = env;
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddMvc();

            services.AddDbContext<UserContext>(options =>
                    options.UseMySql(this.getConnectionString()));

            services.AddTransient<IUserService, UserServiceImpl>();

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
            services.AddResponseCompression();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info {
                    Title = "Documentação da API",
                    Version = "v1",
                    Description = "Descrição da API aqui",
                    Contact = new Contact { Name = "Dhiego", Email = "dhiego.henrique@hotmail.com", Url = "minhaurl.com.br"}
                });

                string caminhoAplicacao =
                    PlatformServices.Default.Application.ApplicationBasePath;
                string nomeAplicacao =
                    PlatformServices.Default.Application.ApplicationName;
                string caminhoXmlDoc =
                    Path.Combine(caminhoAplicacao, $"{nomeAplicacao}.xml");

                c.IncludeXmlComments(caminhoXmlDoc);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseResponseCompression();
            this.enableCors(app);

            if (!env.IsEnvironment("test"))
            {
                this.enableSwagger(app);
            }

            app.UseMvc();
        }

        private IHostingEnvironment CurrentEnvironment { get; set; }

        private void enableCors(IApplicationBuilder app)
        {
            app.UseCors(builder =>
            {
                var corsConfig = Configuration.GetSection("Cors");
                var origins = corsConfig.GetSection("AllowOrigins");
                string[] allowOrigins = origins.Get<string[]>();

                var methods = corsConfig.GetSection("AllowMethods");
                string[] allowMethods = methods.Get<string[]>();

                if (allowOrigins == null || allowOrigins.Count() == 0)
                {
                    builder.AllowAnyOrigin();
                } else
                {
                    builder.WithOrigins(allowOrigins);
                }

                builder.WithMethods(allowMethods);
                builder.AllowAnyHeader();
            });
        }

        private void enableSwagger(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Nome da API");
            });
        }

        private string getConnectionString()
        {
            string connection = Configuration.GetConnectionString("UserContext");
            string envName = CurrentEnvironment.EnvironmentName;
            if (envName == null || !envName.ToLower().Equals("Production".ToLower()))
            {
                return connection;
            }

            connection = Environment.GetEnvironmentVariable("CLEARDB_DATABASE_URL");
            Uri url;
            bool isUrl = Uri.TryCreate(connection, UriKind.Absolute, out url);
            if (isUrl)
            {
                connection = $"Server={url.Host};Uid={url.UserInfo.Split(':')[0]};Pwd={url.UserInfo.Split(':')[1]};Database={url.LocalPath.Substring(1)};pooling=true;";
            }

            return connection;
        }
    }
}
