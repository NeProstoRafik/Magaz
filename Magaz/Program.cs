using Magaz.Data;
using Microsoft.EntityFrameworkCore;

namespace Magaz
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //���������� ������
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession(op =>
            {
                op.IdleTimeout = TimeSpan.FromMinutes(10);
                op.Cookie.HttpOnly = true;
                op.Cookie.IsEssential = true;
            });

            // Add services to the container.
            builder.Services.AddControllersWithViews();


            builder.Services.AddDbContext<Context>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddDefaultIdentity<Microsoft.AspNetCore.Identity.IdentityUser>()
                .AddEntityFrameworkStores<Context>();      //����� ��������� UI �� ����� ������

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
            app.UseSession(); // ��������� �������� �����
                              //app.MapControllerRoute(

            //    name: "default",
            //    pattern: "{controller=Home}/{action=Index}/{id?}");

            app.UseEndpoints(endpoints =>
            {
            endpoints.MapRazorPages();
            endpoints.MapControllerRoute(
                 name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            app.Run();
        }
}
}