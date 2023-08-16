using Magaz.DAL.Data;
using Magaz.DAL.Repository;
using Magaz.DAL.Repository.IRepository;
using Magaz.Models;
using Magaz.Utility;
using Magaz.Utility.BrainTreeSettings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Magaz.DAL.Repository;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace Magaz
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            //для сессии
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession(op =>
            {
                op.IdleTimeout = TimeSpan.FromMinutes(10);
                op.Cookie.HttpOnly = true;
                op.Cookie.IsEssential = true;
            });

            builder.Services.Configure<BrainTreeSettings>(builder.Configuration.GetSection("BrainTree")); // чтобы брать настройки из апсетинга
            builder.Services.AddScoped<IBrainTreeGate, BrainTreeGate>();

            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IApplicationTypeRepository, ApplicationTypeRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IInquiryHeaderRepository, InquiryHeaderRepository>();
            builder.Services.AddScoped<IInquiryDetailRepository, InquiryDetailRepository>();
            builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
            builder.Services.AddScoped<IOrderHeaderRepository, OrderHeaderRepository>();
            builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();

            builder.Services.AddAuthentication().AddFacebook(Options =>
              {
                  Options.AppId = "979166549209276";
                  Options.AppSecret = "0136648c5c6de648ac4d61b2d6534281";
              })
                
                  .AddOAuth("VK", "Vkontakte", config =>
                  {
                      config.ClientId = "51727918";
                      config.ClientSecret = "XAjVPPWzISaTDNvtnGAh";
                      config.ClaimsIssuer = "VKontakte";
                       config.CallbackPath = new PathString("/signin-vkontakte-token");
                    //  config.CallbackPath = new PathString("/Home");
                      config.AuthorizationEndpoint = "https://oauth.vk.com/authorize";
                      config.TokenEndpoint = "https://oauth.vk.com/access_token";
                   //  config.Scope.Add("email");
                     config.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "user_id");
                     config.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");

                     config.SaveTokens = true;
                      config.Events = new OAuthEvents
                      {
                          OnCreatingTicket = context =>
                          {
                              context.RunClaimActions(context.TokenResponse.Response.RootElement);
                              return Task.CompletedTask;
                          },
                      
                      };
                  });
            // Add services to the container.
            builder.Services.AddControllersWithViews();


            builder.Services.AddDbContext<Context>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddIdentity<Microsoft.AspNetCore.Identity.IdentityUser, IdentityRole>()
                .AddDefaultTokenProviders().AddDefaultUI()   //токен и юай
                .AddEntityFrameworkStores<Context>();      //нужно установить UI из нугет пакета

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession(); // мидлваря сессии
                             

            //app.MapControllerRoute(

            //    name: "default",
            //    pattern: "{controller=Home}/{action=Index}/{id?}");

            app.UseEndpoints(endpoints =>
            {
            endpoints.MapRazorPages(); //добавили рейзер страницы для индификации
            endpoints.MapControllerRoute(
                 name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            app.Run();
        }
      

    }
}