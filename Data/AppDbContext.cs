using GooseShop.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
namespace GooseShop.Data
{



    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        // Додаємо ці два рядки:
        public DbSet<CachedWarehouse> CachedWarehouses { get; set; }
        public DbSet<AppConfig> AppConfigs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }

        public DbSet<ProductConstructor> ProductConstructors { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Створюємо індекс для CityRef, щоб пошук за містом був миттєвим
            modelBuilder.Entity<CachedWarehouse>()
                .HasIndex(w => w.CityRef);
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId);
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Чашки" },
                new Category { Id = 2, Name = "Футболки" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Класична біла чашка",
                    Description = "Ідеальна для вашої кави",
                    BasePrice = 250,
                    CategoryId = 1,
                    ImageUrl = "cup_white.jpg"
                },
                new Product
                {
                    Id = 2,
                    Name = "Чорна футболка",
                    Description = "100% бавовна",
                    BasePrice = 450,
                    CategoryId = 2,
                    ImageUrl = "tshirt_black.jpg"
                }
            );
        }
    }
}
