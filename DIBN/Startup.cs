using DIBN.Areas.Admin.Models.ScheduledTasks;
using DIBN.Models.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static DIBN.Areas.Admin.Data.DataSetting;

namespace DIBN
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            TaskSchedulerModel.StartAsync().GetAwaiter().GetResult();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        [Obsolete]
        public void ConfigureServices(IServiceCollection services)
        {
            var con = new Connection();
            Configuration.Bind("ConnectionStrings", con);
            services.AddSingleton(con);
            services.RegisterConfiguration();

            services.AddControllersWithViews();

            services.AddRazorPages();

            services.AddLogging();

            services.AddMvc(options => options.EnableEndpointRouting = false)
            .AddViewOptions(options =>
            {
                options.HtmlHelperOptions.ClientValidationEnabled = false;

            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .AddSessionStateTempDataProvider();

            services.AddSession();

            services.AddResponseCaching();

            services.AddMemoryCache();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/logout";
                options.AccessDeniedPath = "/AccessDenied";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(5);
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AllowView", policy =>
                {
                    policy.RequireAssertion(context =>
                    {
                       List<string> listRolesElements = new List<string>();
                       var userIdentity = (ClaimsIdentity)context.User.Identity;
                        var claims = userIdentity.Claims;
                        var actorClaimType = userIdentity.RoleClaimType;
                        var actor = claims.Where(c => c.Type == ClaimTypes.Role).ToList();
                        if (actor.Count > 0)
                        {
                            for(int i = 0; i < actor.Count; i++)
                            {
                                listRolesElements.Add(actor[i].Value);
                            }
                        }
                        return listRolesElements.Contains("DIBN");
                    });
                });
            });
            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = int.MaxValue;
                options.ValueLengthLimit = int.MaxValue;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/PageNotFound");
                app.UseHsts();
            }

            app.UseAuthorization();

            app.UseAuthentication();
            app.Use(async (context, next) =>
            {
                var username = context.User.Identity.IsAuthenticated ? context.User.Identity.Name : "Guest";
                LogContext.PushProperty("Username", username); //Push user in LogContext;  

                var ip = context.Connection.RemoteIpAddress.ToString();
                LogContext.PushProperty("IP", !String.IsNullOrWhiteSpace(ip) ? ip : "unknown");

                await next();
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/Home/PageNotFound";
                    await next();
                }
                if (context.Response.StatusCode > 500)
                {
                    context.Request.Path = "/Home/PageNotFound";
                    await next();
                }
            });
            
            //app.UseStatusCodePages();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();

            app.UseAuthorization();
            app.Use(async (context, next) =>
            {
                // Forward to the next one.
                await next.Invoke();
            });

            app.UseMvc(routes =>
            {
                routes.MapAreaRoute("areas", "areas", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Account}/{action=Login}/{id?}");
            });
        }
    }
}
