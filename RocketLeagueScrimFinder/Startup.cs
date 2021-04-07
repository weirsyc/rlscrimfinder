using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RocketLeagueScrimFinder.Services;
using RocketLeagueScrimFinder.SignalR;
using RocketLeagueScrimFinder.Models;
using Microsoft.EntityFrameworkCore;

namespace RocketLeagueScrimFinder
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
            services.AddDbContext<ScrimFinderContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("RlsfDatabase")));

            services.AddControllersWithViews().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

#if DEBUG
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
#endif

#if !DEBUG
            services.AddCors(options =>
            {
                options.AddPolicy(name: "_allowedOrigins",
                                  builder =>
                                  {
                                      builder.WithOrigins("https://rlscrimfinder.com").AllowCredentials().AllowAnyHeader();
                                  });
            });
#endif

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/api/authentication/signin";
                options.LogoutPath = "/api/authentication/signout";
            })
            .AddSteam();

            services.AddMemoryCache();
            services.AddSignalR();

            services.AddSingleton<TrackerService>();
            services.AddSingleton<MatchmakingService>();
            services.AddTransient<SchedulingService>();
            services.AddTransient<SteamService>();

#if !DEBUG
            services.AddMvc();
#endif
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

#if DEBUG
            app.UseSpaStaticFiles();
#endif


#if !DEBUG
            app.UseCors("_allowedOrigins");
#endif
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "Management",
                    pattern: "{area:exists}/{controller=Management}/{action=Index}/{id?}");
                endpoints.MapHub<AppHub>("/apphub");
            });

#if DEBUG
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
#endif
        }
    }
}
