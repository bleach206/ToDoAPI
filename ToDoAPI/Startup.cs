using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

using Common;
using Common.Interface;
using Repository;
using Repository.Interface;
using Service;
using Service.Interface;

using Swashbuckle.AspNetCore.Swagger;

namespace ToDoAPI
{
    /// <summary>
    /// use to be gobal middleware pipeline
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }



        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>  
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = "localhost:6379"; //location of redis server
            });
                      
            ConfigureIOC(services);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "To Do API", Version = "v1" });                
                var filePath = Path.Combine(System.AppContext.BaseDirectory, "ToDoAPI.xml");
                c.IncludeXmlComments(filePath);
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param> 
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "To Do API V1");       
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        /// <summary>
        /// setup IOC
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureIOC(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IETagCache, ETagCache>();
            services.AddScoped<IToDoService, ToDoService>();           
            services.AddTransient<IToDoRepository>(repository => new ToDoRepository(Configuration.GetValue<string>("AppSettings:SqlConnection")));           
        }
    }
}
