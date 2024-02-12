using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using diplom2.Data;
using diplom2.Hub;
using diplom2.Logic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace diplom2
{

    static class DBConnect
    {
        public static string _connectionString;
        public static DbContextOptions<Context> options;
    }

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var optionsBuilder = new DbContextOptionsBuilder<Context>();
            DBConnect.options = optionsBuilder.UseNpgsql(Configuration.GetConnectionString("AuthConnection")).Options;
            DBConnect._connectionString = Configuration.GetConnectionString("AuthConnection");

            services.AddCors();
            services.AddDbContext<Context>(opt =>opt.UseNpgsql(Configuration.GetConnectionString("AuthConnection")));

            services.AddSignalR();

            services.AddControllers();

            services.InitSettings(Configuration,null);

            /*services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                options.JsonSerializerOptions.MaxDepth = 0;
            });*/


            /*services.AddControllers()
                 .AddNewtonsoftJson(x =>
                 {
                     x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                 });*/




            services.AddSingleton<Manufacture>(new Manufacture());




        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime hostApplicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //string ip = "localhost";//"192.168.1.167";
            

            app.UseCors(builder => builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowAnyOrigin()
            );


            app.UseFileServer();

            /*
            app.UseCors(options => options
                 .WithOrigins(new[] { "http://" + ip + ":3000", "https://" + ip + ":3000", "http://" + ip + ":8080", "http://" + ip + ":4200" })
                 .AllowAnyHeader()
                 .AllowAnyMethod()
                 .AllowCredentials()
             );
            */

            // app.UseAuthorization();

            /* app.UseCookiePolicy(new CookiePolicyOptions
             {
                 MinimumSameSitePolicy = SameSiteMode.Strict,
                 HttpOnly = HttpOnlyPolicy.Always,
                 Secure = CookieSecurePolicy.Always
             });*/


            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
            };

            app.UseWebSockets(webSocketOptions);

           /* app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        using (WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync())
                        {
                            await Echo(context, webSocket);
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                }
                else
                {
                    await next();
                }

            });*/





            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHub<ChatHub>("/chat")
                        .RequireCors(builder => builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowAnyOrigin());
            });


            hostApplicationLifetime.ApplicationStarted.Register(() =>
            {

                var serviceProvider = app.ApplicationServices;
                var chatHub = (IHubContext<ChatHub>)serviceProvider.GetService(typeof(IHubContext<ChatHub>));

                var timer = new System.Timers.Timer(1000);
                timer.Enabled = true;
                timer.Elapsed += delegate (object sender, System.Timers.ElapsedEventArgs e) {
                    chatHub.Clients.All.SendAsync("setTime", DateTime.Now.ToString("dddd d MMMM yyyy HH:mm:ss"));
                };
                timer.Start();
            });

        }

      

    }
}
