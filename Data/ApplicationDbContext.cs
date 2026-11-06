
using CinemaProject.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieSubImage> MovieSubImages { get; set; }
        public DbSet<CustomerAccount> CustomerAccounts { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Movie>()
                .HasOne(m => m.Category)
                .WithMany(c => c.Movies)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Movie>()
                .HasOne(m => m.Cinema)
                .WithMany(c => c.Movies)
                .HasForeignKey(m => m.CinemaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MovieSubImage>()
                .HasOne(i => i.Movie)
                .WithMany(m => m.SubImages)
                .HasForeignKey(i => i.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Movie>()
                .HasMany(m => m.Actors)
                .WithMany(a => a.Movies);

            modelBuilder.Entity<CustomerAccount>()
                .HasIndex(account => account.NormalizedEmail)
                .IsUnique();

            modelBuilder.Entity<PasswordResetToken>()
                .HasIndex(token => token.TokenHash)
                .IsUnique();

            modelBuilder.Entity<PasswordResetToken>()
                .HasOne(token => token.CustomerAccount)
                .WithMany()
                .HasForeignKey(token => token.CustomerAccountId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
