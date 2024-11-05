using App.Bal.Repositories;
using App.Bal.Services;
using App.Dal;
using App.Entity.Models.Auth;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;

namespace Veribuild_latest
{
    public static class AppService
    {
        const int maxRequestLimit = 209715200; // 200MB

        public static IServiceCollection AddVeriBuildServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddControllersWithViews();
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), option =>
                {
                    option.UseNetTopologySuite();
                });
            });
            services.AddIdentity<AppUser, AppUserRole>(config =>
            {
                config.Password.RequireUppercase = true;
                config.Password.RequireDigit = true;
                config.Password.RequireLowercase = true;
                config.Password.RequireNonAlphanumeric = true;

                config.User.RequireUniqueEmail = true;
                config.SignIn.RequireConfirmedEmail = true;

            }).
            AddEntityFrameworkStores<AppDbContext>().
            AddDefaultTokenProviders();


            services.AddDataProtection().PersistKeysToDbContext<AppDbContext>();
            services.AddAuthentication();
            services.AddHttpClient();
            services.AddAuthorization();

            services.AddSession(config =>
            {
                config.IdleTimeout = TimeSpan.FromHours(2);
                config.Cookie.Path = "/";
                config.Cookie.IsEssential = true;
            });
            services.ConfigureApplicationCookie(config =>
            {
                config.SlidingExpiration = true;
                config.LoginPath = "/";
                config.AccessDeniedPath = "/";
                config.ExpireTimeSpan = TimeSpan.FromHours(2);
                config.Cookie.Path = "/";
            });

            // If using IIS
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = maxRequestLimit;
            });
            // If using Kestrel
            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = maxRequestLimit;
            });
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = maxRequestLimit;
                x.MultipartBodyLengthLimit = maxRequestLimit;
                x.MultipartHeadersLengthLimit = maxRequestLimit;
            });


            // add services to container
            //services.AddScoped<DetailsActionFilter>();
            //services.AddScoped<LoginActionFilter>();

            services.AddTransient<IStorageService, StorageService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IBlockchainService, BlockchainService>();
            services.AddTransient<IPropertyService, PropertyService>();
            services.AddTransient<IMailService, MailService>();

            return services;
        }
    }
}
