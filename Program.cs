
using CinemaProject.Data;
using CinemaProject.Services;
using CinemaProject.Models;
using CinemaProject.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TaskCinema
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "CineVerse.Auth";
                    options.LoginPath = "/Customer/Account/Login";
                    options.AccessDeniedPath = "/Customer/Account/Login";
                    options.ExpireTimeSpan = TimeSpan.FromDays(14);
                    options.SlidingExpiration = true;
                });

            var connectionString = builder.Configuration.GetConnectionString("CinemaProject")
                ?? throw new InvalidOperationException(
                    "Connection string 'CinemaProject' was not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IPublicMovieCatalog, PublicMovieCatalog>();
            builder.Services.AddScoped<IAdminMovieService, AdminMovieService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IMovieImageStorage, LocalMovieImageStorage>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IPasswordHasher<CustomerAccount>, PasswordHasher<CustomerAccount>>();
            builder.Services.AddScoped<IProfileImageStorage, LocalProfileImageStorage>();
            builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
            builder.Services.Configure<SmtpEmailOptions>(builder.Configuration.GetSection("Email:Smtp"));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            if (app.Environment.IsDevelopment())
            {
                await using var scope = app.Services.CreateAsyncScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await dbContext.Database.MigrateAsync();
            }

            app.UseStaticFiles();
            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            await app.RunAsync();
        }
    }
}
