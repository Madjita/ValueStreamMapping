using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using React.AspNet;
using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using DiplomReactNetCore.DAL.Context;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;

using DiplomReactNetCore.DAL.Models.DataBase;
using DiplomReactNetCore.L.Simulation;

namespace DiplomReactNetCore
{


    public class Startup
    {
        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {

            string connection = Configuration.GetConnectionString("SqliteConnection");


            var contextOptions = new DbContextOptionsBuilder<MyContext>()
                       .UseSqlite(connection)
                       .Options;



            using (MyContext _context = new MyContext(contextOptions))
            {
                _context.Init();
            }
            services.AddDbContext<MyContext>(o => o.UseSqlite(connection));

            Action<Simulation> mduOptions = (opt =>{});
            services.Configure(mduOptions);
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<Simulation>>().Value);


            //
            Manufacture manufacture = new Manufacture(connection);
            services.AddSingleton<Manufacture>(manufacture);
            //

            services.AddJsEngineSwitcher(options => options.DefaultEngineName = ChakraCoreJsEngine.EngineName)
            .AddChakraCore();

            // existing services below:
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddReact();
            services.AddMvc();

            // Add Cors
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));


            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddControllers();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseReact(config => { });
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Enable Cors
            app.UseCors("MyPolicy");

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto
            });

        }


        /*public void init(MyContext _context)
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _context.BufferVSM.Add(
                new BufferVSM
                {
                    Name = "Кофе",
                    Type = "гр",
                    MinHold = 100,
                    Max = 1000,
                    Value = 150,
                    ReplenishmentSec = 60,
                    Parallel = false,
                });

            _context.BufferVSM.Add(
                new BufferVSM
                {
                    Name = "Фильтры",
                    Type = "шт",
                    MinHold = 1,
                    Max = 10,
                    Value = 2,
                    ReplenishmentSec = 300,
                    Parallel = false,
                });

            _context.BufferVSM.Add(
                new BufferVSM
                {
                    Name = "Вода",
                    Type = "л",
                    MinHold = 0,
                    Max = 999,
                    Value = 100,
                    ReplenishmentSec = 600,
                    Parallel = false,
                });

            _context.SaveChanges();
            
        }*/

    }
}
