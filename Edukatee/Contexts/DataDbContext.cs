using Edukatee.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;

namespace Edukatee.Contexts
{
    public class DataDbContext : IdentityDbContext
    {
        public DataDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Slider>Sliders { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public override Task<int>SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            IEnumerable<EntityEntry<Slider>> entries = ChangeTracker.Entries<Slider>();
            foreach (EntityEntry<Slider>entry in entries)
            {
                if(entry.State == EntityState.Added)
                {
                    DateTime dateTime = DateTime.UtcNow;
                    DateTime azTime = dateTime.AddHours(4);
                    entry.Entity.CreatedTime = azTime;
                    entry.Entity.UpdatedTime = null;
                }else if(entry.State == EntityState.Modified)
                {
                    DateTime dateTime = DateTime.UtcNow;
                    DateTime azTime = dateTime.AddHours(4);
                    entry.Entity.UpdatedTime = azTime;
                    var modifiedProperties = entry.Properties.Where(prop => prop.IsModified && !prop.Metadata.IsPrimaryKey());
                    if(!modifiedProperties.Any()) 
                    {
                        entry.Entity.UpdatedTime = null;
                    }
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Setting>().HasData(new Setting
            {
                Id = 1,
                Logo = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ2zPJA81k7VuodnK3w4LaZqMJGjVuvedNKU365l2xQQQ&s",
                Address = "Baki 34st",
                Email = "Fuad@mai.ru",
                PhoneNumber = "12345671"
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
