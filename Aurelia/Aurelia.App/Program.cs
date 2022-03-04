using Aurelia.App.Data;
using Aurelia.App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Aurelia.App
{
    public class Program 
    {
        public static void Main(string[] args) 
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddMvc();
            builder.Services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddDefaultIdentity<AureliaUser>(options => options.SignIn.RequireConfirmedAccount = false).AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();
            var app = builder.Build();
            CreateUserAndRoles(app);
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
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

            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

           

            app.Run();

        }
        public static void CreateUserAndRoles(WebApplication app)

        {
            using (var scope = app.Services.CreateScope())
            {
                using (var aureliaDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>()) 
                {
                    if (!aureliaDbContext.Roles.Any()) 
                    {
                        IdentityRole superadmin = new IdentityRole { Name = "SuperAdmin", NormalizedName = "SUPERADMIN" };
                        IdentityRole user = new IdentityRole { Name = "User", NormalizedName = "USER" };

                        aureliaDbContext.Roles.Add(superadmin);
                        aureliaDbContext.Roles.Add(user);
                    }
                    
                    aureliaDbContext.SaveChanges();
                }
            
            }   
     
        }
    }

}
