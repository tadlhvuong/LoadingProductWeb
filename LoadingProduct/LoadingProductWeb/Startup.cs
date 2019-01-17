using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TCVShared.Data;
using Microsoft.AspNetCore.HttpOverrides;
using TCVShared.Helpers;

namespace TCVWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDBContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("MySQLConnection"), b => b.MigrationsAssembly("TCVWeb")));

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddIdentity<AppUser, AppRole>(config =>
            {
                config.User.RequireUniqueEmail = false;
                config.Password.RequireDigit = false;
                config.Password.RequireLowercase = false;
                config.Password.RequireUppercase = false;
                config.Password.RequiredUniqueChars = 0;
                config.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<AppDBContext>().AddDefaultTokenProviders();

            services.AddAuthentication().AddFacebook(options =>
            {
                options.AppId = Configuration["Authentication:Facebook:AppId"];
                options.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                options.SaveTokens = true;
            });

            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                options.SaveTokens = true;
            });

            //services.AddAuthMessageSender(options =>
            //{
            //    options.SpeedSmsToken = Configuration["AuthMessage:SpeedSmsToken"];
            //    options.SendGridApiKey = Configuration["AuthMessage:SendGridApiKey"];
            //    options.SendGridSenderName = Configuration["AuthMessage:SendGridSenderName"];
            //    options.SendGridSenderEmail = Configuration["AuthMessage:SendGridSenderEmail"];
            //});

            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.IdleTimeout = TimeSpan.FromMinutes(60);
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });


            app.UseStaticFiles();
            app.UseAuthentication();
            app.InitAppSettings();
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                      name: "areas",
                      template: "{area:exists}/{controller=HomeAdmin}/{action=Index}/{id?}"
                    );
                //routes.MapRoute(
                //    name: "areaRoute",
                //    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
