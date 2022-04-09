using AspNetCore.ReCaptcha;
using Aurelia.App.Data;
using Aurelia.App.Models;
using Aurelia.App.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Stripe;

namespace Aurelia.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var emailConfig = builder.Configuration
            .GetSection("GmailConf")
            .Get<GmailConf>();

            builder.Services.AddSingleton(emailConfig);

            builder.Services.AddTransient<IEmailService, EmailService>();
            builder.Services.AddTransient<IEmailSender, EmailSender>(i =>
                new EmailSender(
                    builder.Configuration["GmailConf:SmtpClient"],
                    builder.Configuration.GetValue<int>("GmailConf:Port"),
                    enableSSL: true,
                    builder.Configuration["GmailConf:Username"],
                    builder.Configuration["GmailConf:Password"]
                )
            );
            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddMvc();
            builder.Services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddSingleton<ChargeService>(new ChargeService());
            StripeConfiguration.SetApiKey(builder.Configuration["Stripe:TestSecretKey"]);

            builder.Services.AddReCaptcha(builder.Configuration.GetSection("ReCaptcha"));

            builder.Services.Configure<DataProtectionTokenProviderOptions>(o =>
             o.TokenLifespan = TimeSpan.FromMinutes(15));

            builder.Services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                options.SignInScheme = IdentityConstants.ExternalScheme;
            });
            builder.Services.AddAuthentication().AddFacebook(options =>
            {
                options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
                options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
                options.AccessDeniedPath = "/AccessDeniedPathInfo";
            });
            builder.Services.AddDefaultIdentity<AureliaUser>(options => {
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedAccount = true;
                options.User.RequireUniqueEmail = true;
            })
                .AddRoles<IdentityRole>()
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
            app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

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