using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NovelSite.Data.Identity;

namespace NovelSite.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationIdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Создание уникальных полей
            builder.Entity<ApplicationIdentityUser>(entity =>
            {
                entity.HasIndex(e => e.UserName).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseNpgsql("Server=localhost;Port=5432;User Id=postgres;Password=postgres;Database=novelsite;");
            optionsBuilder.UseNpgsql("Server=localhost;Port=5432;User Id=postgres;Password=moloko990;Database=novelsite;");
            //optionsBuilder.UseLazyLoadingProxies();
        }
    }
}