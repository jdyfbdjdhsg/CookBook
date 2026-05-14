using CookBooks.Models;
using Microsoft.EntityFrameworkCore;

namespace CookBooks.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Recipe> Recipes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Уникальный email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Добавляем индекс для роли (для быстрого поиска админов)
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Role);

            // Связь User -> Recipes
            modelBuilder.Entity<Recipe>()
                .HasOne(r => r.Author)
                .WithMany()
                .HasForeignKey(r => r.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}