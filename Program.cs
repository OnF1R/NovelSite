using Amazon.Runtime;
using Amazon.S3;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using NovelSite.Keys;
using NovelSite.Data;
using NovelSite.Data.Identity;
using NovelSite.Services.Email;
using NovelSite.Services.S3.Implimentations;
using NovelSite.Services.S3.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace NovelSite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<IAmazonS3>(sp =>
            {
                var awsCredentials = new BasicAWSCredentials(SelecterS3Keys.AccessKey, SelecterS3Keys.SecretKey);
                var config = new AmazonS3Config
                {
                    ServiceURL = "https://s3.ru-1.storage.selcloud.ru",
                    ForcePathStyle = true
                };
                return new AmazonS3Client(awsCredentials, config);
            });

            builder.Services.AddTransient<IFileService, FileService>();

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            
            Console.WriteLine(connectionString);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<ApplicationIdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddControllersWithViews();

            builder.Services.AddTransient<IEmailSender, EmailSender>();

            var app = builder.Build();

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

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}");

            app.MapRazorPages();

            app.Run();
        }
    }
}